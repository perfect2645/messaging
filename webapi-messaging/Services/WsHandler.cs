using Logging;
using System.Net.WebSockets;
using System.Text;
using webapi_messaging.Interfaces.Services;

namespace webapi_messaging.Services
{
    public class WsHandler : IWsHandler
    {
        public async void HandleWebSocket(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 16];
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                        break;
                    }
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Log4Logger.Logger.Info($"WsHandler::Received message: {message}");

                    // Echo the message back to the client
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Echo from server")), WebSocketMessageType.Text, result.EndOfMessage, CancellationToken.None);
                }
            }
            catch (WebSocketException ex)
            {
                Log4Logger.Logger.Error($"WsHandler::WebSocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log4Logger.Logger.Error($"WsHandler::Exception: {ex.Message}");
            }
            finally
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).Wait();
                }
            }
        }
    }
}
