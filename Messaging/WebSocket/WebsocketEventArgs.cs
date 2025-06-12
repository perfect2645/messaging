using Messaging.Exceptions;

namespace Messaging.WebSocket
{
    public class WebsocketEventArgs
    {
        public string? Message { get; }
        public string? ConnectionId { get; set; }
        public string? ServerUrl { get; set; }
        public WsException? Exception { get; set; }

        public WebsocketEventArgs(string message, string? connectionId, string? serverUrl)
        {
            Message = message;
            ConnectionId = connectionId;
            ServerUrl = serverUrl;
        }

        public WebsocketEventArgs(WsException exception, string message, string? connectionId, string? serverUrl)
        {
            Exception = exception;
            Message = message;
            ConnectionId = connectionId;
            ServerUrl = serverUrl;
        }
    }
}
