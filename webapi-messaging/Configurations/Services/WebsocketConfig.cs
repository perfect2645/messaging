using webapi_messaging.Interfaces.Services;

namespace webapi_messaging.Configurations.Services
{
    public static class WebsocketConfig
    {
        public static void ConfigureWebsocket(this WebApplication app)
        {
            app.UseWebSockets();
            app.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    using var websocket = await context.WebSockets.AcceptWebSocketAsync();
                    var websocketHandler = context.RequestServices.GetRequiredService<IWsHandler>();
                    websocketHandler.HandleWebSocket(context, websocket);
                }
                else
                {
                    context.Response.StatusCode = 400; // Bad Request
                }
            });
        }
    }
}
