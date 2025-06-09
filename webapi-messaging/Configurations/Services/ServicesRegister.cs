using Microsoft.OpenApi.Expressions;

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
                var serverUrl = config.ReadString
            });
        }
    }
}
