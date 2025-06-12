using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Config
{
    public static class Initializer
    {
        public static void InitWsIoc(this IServiceCollection services)
        {
            services.RegisterKeyedWsClient(Constants.IocClient);
        }

        public static void RegisterKeyedWsClient(this IServiceCollection services,
            string key)
        {
            //services.AddKeyedSingleton<IWsClient>(sp =>
            //{
            //    var config = sp.GetRequiredService<IConfiguration>();
            //    var serverUrl = config.ReadString($"{Constants.Websocket}:{Constants.ServerUrl}");
            //    if (string.IsNullOrEmpty(serverUrl))
            //    {
            //        throw new ArgumentNullException(nameof(serverUrl), "Server URL cannot be null or empty.");
            //    }
            //    return new SignalRClient(serverUrl);
            //}).Keyed<ISignalRClient>(key);
        }
    }
}
