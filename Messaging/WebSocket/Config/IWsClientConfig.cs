namespace Messaging.WebSocket.Config
{
    public interface IWsClientConfig
    {
        string ServerUrl { get; set; }
        string Topic { get; set; }
        bool SkipRegister { get; set; }
        bool EnableLogging { get; set; }
    }
}
