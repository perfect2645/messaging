namespace Messaging.WebSocket
{
    internal interface IReconnectable
    {
        int ReconnectInterval { get; set; }
        int MaxReconnectInterval { get; set; }
        bool IsManualDisconnect { get; set; }
    }
}
