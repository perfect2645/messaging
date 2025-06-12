using System.Net.WebSockets;

namespace Messaging.Interfaces
{
    public interface IWebsocketClient
    {
        ClientWebSocket? ClientWebSocket { get; }
        Task SendMessage(string message);
        Task ReceiveMessages();
        string ServerUrl { get; }
        string Topic { get; set; }
        string? UserId { get; set; }
        string? ConnectionId { get; set; }
        bool SslEnabled { get; }
        bool EnableLogging { get; set; }
        int ReceiveBufferSize { get; set; }
        int SendBufferSize { get; set; }

        event EventHandler<string> OnError;
        event EventHandler<string> OnConnected;
        event EventHandler<string> OnMessageReceived;
    }
}
