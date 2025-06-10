using Messaging.Interfaces;
using Messaging.SignalR;
using Microsoft.OpenApi.Expressions;
using Util;
using webapi_messaging.Const;

namespace webapi_messaging.Configurations.Services
{
    public static class ServicesRegister
    {
        public static void Register(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddControllers();
            //services.AddControllers(option =>
            //{
            //    option.Filters.Add<ExceptionFilterAttribute>();
            //}).AddJsonOptions(option =>
            //{
            //    option.JsonSerializerOptions.Converters.Add(new QueryExpressionValueConverter());
            //});
            services.AddTransient<ISignalRClient, SignalRClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var serverUrl = config.ReadString($"{Constants.Websocket}:{Constants.ServerUrl}");
                if (string.IsNullOrEmpty(serverUrl))
                {
                    throw new ArgumentNullException(nameof(serverUrl), "Server URL cannot be null or empty.");
                }

                return new SignalRClient(serverUrl);
            });
        }
    }
}
