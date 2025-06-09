using Messaging.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.SignalR
{
    public class SignalRClient : ISignalRClient
    {
        public HubConnection? HubConnection { get; private set; }
        public string? ServerUrl { get; set; }
        private Uri? _serverUrl;

        public SignalRClient(string url)
        {
            ServerUrl = url;
            _serverUrl = new Uri(ServerUrl);
            BuildConnection();
        }
        public void BuildConnection()
        {
            if (string.IsNullOrEmpty(ServerUrl))
            {
                throw new ArgumentNullException(nameof(ServerUrl), "Server URL cannot be null or empty.");
            }

            HubConnection = new HubConnectionBuilder()
                .WithUrl(_serverUrl, options =>
                {
                    options.WebSocketConfiguration = webSocketOptions =>
                    {
                        webSocketOptions.KeepAliveInterval = TimeSpan.FromSeconds(30);
                        webSocketOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true; // Accept all certificates (not recommended for production)
                    };
                })
                .Build();
        }
    }
}
