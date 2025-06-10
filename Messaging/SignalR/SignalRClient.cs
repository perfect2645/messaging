using Logging;
using Messaging.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.SignalR
{
    public class SignalRClient : ISignalRClient
    {
        public HubConnection? HubConnection { get; private set; }
        public string? ServerUrl { get; set; }

        private Uri? _serverUrl;

        public event Action<string, object>? OnMessageReceived;

        public SignalRClient(string url)
        {
            ServerUrl = url;
            _serverUrl = new Uri(ServerUrl);
            BuildConnection();
        }
        public void BuildConnection()
        {
            if (string.IsNullOrEmpty(ServerUrl))
            {
                throw new ArgumentNullException(nameof(ServerUrl), "Server URL cannot be null or empty.");
            }

            HubConnection = new HubConnectionBuilder()
                .WithUrl(ServerUrl, options =>
                {
                    options.WebSocketConfiguration = webSocketOptions =>
                    {
                        webSocketOptions.KeepAliveInterval = TimeSpan.FromSeconds(30);
                        webSocketOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Accept all certificates (not recommended for production)
                        if (_serverUrl?.Scheme?.ToLower() == "https")
                        {
                            // Use secure connection settings
                        }
                    };
                })
                .WithAutomaticReconnect([TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)])
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            HubConnection.On<string, object>("ReceiveMessage", (method, message) =>
            {
                Log4Logger.Logger.Info($"SignalRClient: Received message '{message}' using method '{method}'.");
            });

            HubConnection.Closed += async (error) =>
            {
                Log4Logger.Logger.Error($"SignalRClient: Connection closed with error: {error?.Message}");
                await Task.Delay(TimeSpan.FromSeconds(5)); // Wait before reconnecting
                await HubConnection.StartAsync();
            };

            HubConnection.Reconnecting += (error) =>
            {
                Log4Logger.Logger.Error($"SignalRClient: Reconnecting due to error: {error?.Message}");
                return Task.CompletedTask;
            };
        }

        public async void Connect()
        {
            if (HubConnection == null)
            {
                throw new InvalidOperationException("SignalRClient::Connect:HubConnection is not initialized. Call BuildConnection() first.");
            }

            if (HubConnection.State == HubConnectionState.Connected)
            {
                Log4Logger.Logger.Info($"SignalRClient: Already connected to {ServerUrl}");
                return;
            }

            try
            {
                await HubConnection.StartAsync();
                Log4Logger.Logger.Info($"SignalRClient: Connected to {ServerUrl}");
            }
            catch (Exception ex)
            {
                Log4Logger.Logger.Error($"SignalRClient: Error connecting to {ServerUrl}: {ex.Message}");
                throw;
            }
        }
        public void Dicconnect()
        {
            HubConnection?.DisposeAsync();
        }

        public void Send(string message)
        {
            if (HubConnection == null)
            {
                throw new InvalidOperationException("SignalRClient::Send:HubConnection is not initialized. Call BuildConnection() first.");
            }
            if (HubConnection.State != HubConnectionState.Connected)
            {
                Log4Logger.Logger.Warn($"SignalRClient: Cannot send message '{message}' because the connection is not established.");
                return;
            }
            HubConnection.SendAsync("SendStringAsync", "ReceiveMessage", message)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Log4Logger.Logger.Error($"SignalRClient: Error sending message '{message}': {task.Exception?.GetBaseException().Message}");
                    }
                    else
                    {
                        Log4Logger.Logger.Info($"SignalRClient: Message '{message}' sent successfully.");
                    }
                });
        }

        public void MessageReceivedHandler<T>(string method, T message) where T : notnull
        {
            if (HubConnection == null)
            {
                throw new InvalidOperationException("SignalRClient::MessageReceivedHandler:HubConnection is not initialized. Call BuildConnection() first.");
            }
            if (HubConnection.State != HubConnectionState.Connected)
            {
                Log4Logger.Logger.Warn($"SignalRClient: Cannot handle message '{message}' because the connection is not established.");
                return;
            }

            HubConnection.On<string>(method, (msg) =>
            {
                Log4Logger.Logger.Info($"SignalRClient: Received message '{msg}' using method '{method}'.");
                OnMessageReceived?.Invoke(method, msg);
            });

            //HubConnection.InvokeAsync(method, message)
            //    .ContinueWith(task =>
            //    {
            //        if (task.IsFaulted)
            //        {
            //            Log4Logger.Logger.Error($"SignalRClient: Error handling message '{message}': {task.Exception?.GetBaseException().Message}");
            //        }
            //        else
            //        {
            //            Log4Logger.Logger.Info($"SignalRClient: Message '{message}' handled successfully using method '{method}'.");
            //        }
            //    });
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}
