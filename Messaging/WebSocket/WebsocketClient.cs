using Logging;
using Messaging.Config;
using Messaging.Exceptions;
using Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Text;
using Utils;

namespace Messaging.WebSocket
{
    public class WebsocketClient : IWebsocketClient, IReconnectable
    {
        #region Properties  

        public ClientWebSocket? ClientWebSocket { get; private set; }
        public required string ServerUrl { get; init; }
        public required string Topic { get; init; }
        public string? UserId { get; set; }
        public string? ConnectionId { get; set; }
        public bool SslEnabled { get; private set; }
        public bool EnableLogging { get; set; } = true;
        public int ReceiveBufferSize { get; set; } = 1024 * 4; // Default buffer size
        public int SendBufferSize { get; set; } = 1024 * 4; // Default buffer size
        public event EventHandler<WebsocketEventArgs>? OnError;
        public event EventHandler<WebsocketEventArgs>? OnConnected;
        public event EventHandler<WebsocketEventArgs>? OnMessageReceived;

        public int ReconnectInterval { get; set; }
        public int MaxReconnectInterval { get; set; } = 60000 * 10; // Default to 10 minutes
        public bool IsManualDisconnect { get; set; }

        private Uri? _uri;
        private Task? _connectionTask;
        private CancellationTokenSource? _cancellationTokenSource;

        #endregion Properties

        #region Constructor

        public WebsocketClient()
        {
            UserId = Environment.MachineName;
            BuildUri();
            SetupReconnect();

        }

        private void BuildUri()
        {
            try
            {
                ConnectionId = $"{UserId}-{Process.GetCurrentProcess().Id}-{Topic}";
                _uri = new Uri($"{ServerUrl}?connectionId={ConnectionId}&topic={Topic}");
                if (_uri.Scheme.Equals("wss", StringComparison.OrdinalIgnoreCase))
                {
                    SslEnabled = true;
                }
            }
            catch (Exception ex)
            {
                throw new WsException($"WebsocketClient::{ServerUrl}?connectionId={ConnectionId}&topic={Topic} BuildUri failed: {ex.Message}", WsErrCode.BuildClient);
            }
        }

        #endregion Constructor

        #region Connection

        private void SetupReconnect()
        {
            ReconnectInterval = 5000;
            IsManualDisconnect = false;
        }

        private void BuildClient()
        {
            try
            {
                ClientWebSocket?.Abort();
                ClientWebSocket = new ClientWebSocket();
                ClientWebSocket.Options.SetBuffer(ReceiveBufferSize, SendBufferSize);

                _cancellationTokenSource = new CancellationTokenSource();

                if (SslEnabled)
                {
                    Log4Logger.Logger.Debug($"WebsocketClient::[{ServerUrl}][{ConnectionId}] SSL enabled");
                }

                _connectionTask = ConnectToServer();
            }
            catch (WebSocketException ex)
            {
                throw new WsException($"WebsocketClient::BuildClient [{ServerUrl}][{ConnectionId}] failed: {ex.Message}", WsErrCode.BuildClient);
            }
            catch (AuthenticationException ex)
            {
                throw new WsException($"WebsocketClient::BuildClient [{ServerUrl}][{ConnectionId}] Authentication failed: {ex.Message}", WsErrCode.BuildClient);
            }
            catch (Exception ex)
            {
                throw new WsException($"WebsocketClient::BuildClient [{ServerUrl}][{ConnectionId}] failed: {ex.Message}", WsErrCode.BuildClient);
            }
        }

        private async Task ConnectToServer()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            if (_uri == null)
            {
                throw new WsException($"WebsocketClient::ConnectToServer failed: URI is null. url=[{ServerUrl}], Topic=[{Topic}]", WsErrCode.Connection);
            }

            try
            {
                await ClientWebSocket!.ConnectAsync(_uri, _cancellationTokenSource.Token);
                
                Log4Logger.Logger.Info($"WebsocketClient::Connection [{ServerUrl}][{ConnectionId}] connected.");

                SetupReconnect();
                await OnWsConnectedAsync();
            }
            catch (OperationCanceledException ex)
            {
                Log4Logger.Logger.Warn($"WebsocketClient::ConnectToServer [{ServerUrl}][{ConnectionId}] connection cancelled: {ex.Message}");
            }
            catch (WebSocketException ex)
            {
                var errorMessage = $"WebsocketClient::ConnectToServer [{ServerUrl}][{ConnectionId}] failed. [{ex.ErrorCode}]: {ex.Message}";

                var wsException = new WsException(ex, errorMessage, WsErrCode.Connection);
                if (wsException.ErrCode == WsErrCode.ConnectionHeaderInvalid)
                {
                    Log4Logger.Logger.Info(wsException.GetDetailedMessages(), wsException);
                    //.net framework connection response header 'keep-alive' issue backoff
                    SetupReconnect();
                    ReconnectAsync();
                    return;
                }

                OnWsError(wsException);
            }
        }

        private async Task ReconnectIntervalTick()
        {
            await Task.Delay(ReconnectInterval);
            ReconnectInterval = Math.Min(ReconnectInterval * 2, MaxReconnectInterval);
        }

        private async Task ReconnectAsync()
        {
            if (ClientWebSocket?.State == WebSocketState.Open)
            {
                return;
            }
            await ReconnectIntervalTick();
            BuildClient();
        }

        private async Task OnWsConnectedAsync()
        {
            try
            {
                var args = new WebsocketEventArgs("Connected to server.", ConnectionId, ServerUrl);
                OnConnected?.Invoke(this, args);
                await ReceiveMessages();
            }
            catch (Exception ex)
            {
                Log4Logger.Logger.Error($"WebsocketClient::OnWsConnectedAsync [{ServerUrl}][{ConnectionId}] failed: {ex.Message}", ex);
                OnWsError(new WsException(ex, $"WebsocketClient::OnWsConnectedAsync failed: {ex.Message}", WsErrCode.Connection));
            }
        }

        private void OnWsError(WsException wsEx, bool isLogError = true)
        {
            try
            {
                if (isLogError)
                {
                    Log4Logger.Logger.Error(wsEx.GetDetailedMessages(), wsEx);
                }
                var args = new WebsocketEventArgs(wsEx, "OnWsError", ConnectionId, ServerUrl);
                OnError?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                throw new WsException(ex, $"WebsocketClient::OnWsError [{ServerUrl}][{ConnectionId}]", WsErrCode.Exception);
            }
        }

        #endregion Connection

        #region Send

        public async Task SendMessage(string message)
        {
            await _connectionTask!;


            if (ClientWebSocket == null || ClientWebSocket.State != WebSocketState.Open)
            {
                throw new WsException($"WebsocketClient::SendAsync [{ServerUrl}][{ConnectionId}] failed: WebSocket is not connected.", WsErrCode.SendMessage);
            }
            try
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);
                await ClientWebSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);

                Log4Logger.Logger.Info($"WebsocketClient::SendAsync [{ServerUrl}][{ConnectionId}] message sent: {message}");
            }
            catch (Exception ex)
            {
                OnWsError(new WsException(ex, $"WebsocketClient::SendAsync [{ServerUrl}][{ConnectionId}] failed: {ex.Message}", WsErrCode.SendMessage));
            }
        }

        #endregion Send

        #region Receive

        public async Task ReceiveMessages()
        {
            if (ClientWebSocket == null)
            {
                throw new WsException($"WebsocketClient::ReceiveMessages [{ServerUrl}][{ConnectionId}] failed: ClientWebSocket is null.", WsErrCode.ReceiveMessage);
            }

            if (ClientWebSocket.State == WebSocketState.Connecting)
            {
                Log4Logger.Logger.Debug($"WebsocketClient::[{ConnectionId}] is connecting to server");
            }
            else if (ClientWebSocket.State == WebSocketState.Open)
            {
                Log4Logger.Logger.Debug($"WebsocketClient::[{ConnectionId}] connected to server [{ServerUrl}].");
            }

            var buffer = System.Net.WebSockets.WebSocket.CreateClientBuffer(ReceiveBufferSize, SendBufferSize);
            try
            {
                while (ClientWebSocket.State == WebSocketState.Open)
                {
                    var result = await ClientWebSocket.ReceiveAsync(buffer, _cancellationTokenSource.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Log4Logger.Logger.Info($"WebsocketClient::[{ConnectionId}] received close message from server. Closing connection.");
                        await ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Connection [{ConnectionId}] closed", _cancellationTokenSource.Token);
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);
                    if (EnableLogging)
                    {
                        LoggerMessage(message);
                    }
                    OnWsMessageReceived(message);
                }
            }
            catch (WebSocketException ex)
            {
                Log4Logger.Logger.Error($"WebsocketClient::ReceiveMessages [{ServerUrl}][{ConnectionId}] WebSocketException: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new WsException(ex, $"WebsocketClient::ReceiveMessages [{ServerUrl}][{ConnectionId}] failed: {ex.Message}", WsErrCode.ReceiveMessage);
            }
        }

        private void OnWsMessageReceived(string message)
        {
            try
            {
                var args = new WebsocketEventArgs(message, ConnectionId, ServerUrl);
                OnMessageReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                var wsEx = new WsException(ex, $"WebsocketClient::OnMessageReceived failed: {ex.Message}", WsErrCode.ReceiveMessage);
                OnWsError(wsEx, true);
            }
        }

        private void LoggerMessage(string message)
        {
            Log4Logger.Logger.Info($"WebsocketClient::[{ConnectionId}] received message: {message}");
        }

        #endregion Receive

        #region Disconnect

        public void Disconnect()
        {
            IsManualDisconnect = true;
            _cancellationTokenSource?.Cancel();
            if (ClientWebSocket == null)
            {
                return;
            }
            if (ClientWebSocket.State != WebSocketState.Open)
            {
                return;
            }
            ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Connection:[{ConnectionId}] closed manually", CancellationToken.None);

            ClientWebSocket.Dispose();
        }

        #endregion Disconnect

    }
}
