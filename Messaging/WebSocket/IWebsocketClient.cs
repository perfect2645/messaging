using System.Net.WebSockets;

namespace Messaging.WebSocket
{
    public interface IWebsocketClient
    {
        ClientWebSocket? ClientWebSocket { get; }
        Task SendMessage(string message);
        Task ReceiveMessages();
        string ServerUrl { get; }
        string Topic { get; }
        string? UserId { get; set; }
        string? ConnectionId { get; set; }
        bool SslEnabled { get; }
        bool EnableLogging { get; set; }
        int ReceiveBufferSize { get; set; }
        int SendBufferSize { get; set; }

        event EventHandler<WebsocketEventArgs> OnError;
        event EventHandler<WebsocketEventArgs> OnConnected;
        event EventHandler<WebsocketEventArgs> OnMessageReceived;
    }
}
