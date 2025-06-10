using Microsoft.AspNetCore.SignalR.Client;

namespace Messaging.Interfaces
{
    public interface ISignalRClient
    {
        string? ServerUrl { get; set; }
        HubConnection? HubConnection { get; }
        void BuildConnection();
        void Connect();
        void Disconnect();
        void Send(string message);
        event Action<string, object>? OnMessageReceived;

    }
}
