using Messaging.SignalR;

namespace webapi_messaging.Configurations.Services
{
    public static class SignalRConfig
    {


        public static void ConfigSignalR(this WebApplication app)
        {
            app.MapHub<SignalRHub>("/signalrHub");
        }

        public static void RegisterSignalR(this IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = 102400; // 100 KB
            });
        }
    }
}
