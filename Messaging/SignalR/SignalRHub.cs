using Logging;
using Microsoft.AspNetCore.SignalR;

namespace Messaging.SignalR
{
    public class SignalRHub : Hub
    {
        public async Task SendStringAsync(string method, string message)
        {
            await Clients.All.SendAsync(method, message);
            Log4Logger.Logger.Info($"SignalRHub: Sent message '{message}' using method '{method}' to all clients.");
        }
    }
}
