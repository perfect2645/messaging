namespace webapi_messaging.Configurations.Services
{
    public static class SignalRConfig
    {
        public static void AddSignalR(this IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = 102400; // 100 KB
            });
        }

        public static void MapSignalR(this WebApplication app)
        {
            app.MapHub<SignalRHub>("/signalrHub");
        }
    }
}
