using System.Net.WebSockets;

namespace webapi_messaging.Interfaces.Services
{
    public interface IWsHandler
    {
        void HandleWebSocket(HttpContext context, WebSocket webSocket);
    }
}
