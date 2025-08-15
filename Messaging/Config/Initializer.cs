using Messaging.Exceptions;
using Messaging.Interfaces;
using Messaging.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Config
{
    public static class Initializer
    {
        public static void InitWsIoc(this IServiceCollection services, IConfiguration config)
        {
            //services.RegisterKeyedWsClient(Constants.IocClient);
        }

        public static void RegisterKeyedWsClient(this IServiceCollection services,
            IWsClientConfig config)
        {
            try
            {
                if (string.IsNullOrEmpty(config.ServerUrl))
                {
                    throw new WsException($"RegisterKeyedWsClient [{config.Topic}]: ServerUrl is null or empty.", WsErrCode.Config);
                }
                if (string.IsNullOrEmpty(config.Topic))
                {
                    throw new WsException($"RegisterKeyedWsClient [{config.ServerUrl}]: Topic is null or empty.", WsErrCode.Config);
                }

                var key = config.Topic.ToLower();
                services.AddKeyedSingleton<IWebsocketClient>(key, (sp, args) =>
                {
                    var client = new WebsocketClient() 
                    { 
                        ServerUrl = config.ServerUrl,
                        Topic = key,
                        EnableLogging = config.EnableLogging,
                    };

                    return client;
                });
            }
            catch (Exception ex)
            {
                throw new WsException(ex, $"RegisterKeyedWsClient:[{config.Topic}] failed", WsErrCode.Register);
            }
        }
    }
}
