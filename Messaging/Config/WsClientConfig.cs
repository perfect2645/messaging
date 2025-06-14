namespace Messaging.Config
{
    public class WsClientConfig : IWsClientConfig
    {
        public required string ServerUrl { get; set; }
        public required string Topic { get; set; }
        public bool SkipRegister { get; set; }
        public bool EnableLogging { get; set; } = true;

    }
}
