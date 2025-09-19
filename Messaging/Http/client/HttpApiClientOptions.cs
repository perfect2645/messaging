using Microsoft.Net.Http.Headers;

namespace Messaging.Http.client
{
    public class HttpApiClientOptions
    {
        public Uri? BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        public Dictionary<string, string> DefaultHeaders { get; set; } = new()
        {
            { HeaderNames.Accept, "application/json" },
            { HeaderNames.UserAgent, "HttpApiClient/1.0" }
        };

        public bool EnableRetry { get; set; } = true;
        public int MaxRetryCount { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    }
}
