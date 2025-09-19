using Logging;
using Messaging.Http.client;
using Messaging.Http.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Messaging.Http.helper
{
    public static class HttpApiClientExt
    {
        public static IServiceCollection AddHttpApiClient(
            this IServiceCollection services,
            string name,
            Action<HttpApiClientOptions> configureOptions)
        {
            if (services == null)
            {
                throw new HttpException($"HttpClient Register error. Ioc Service :[{nameof(services)}] is null", HttpStatus.ClientRegister);
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new HttpException($"HttpClient Register error. Client name is empty.", HttpStatus.ClientRegister);
            }
            
            if (configureOptions == null)
            {
                throw new HttpException($"HttpClient Register error. Client configureOptions is empty.", HttpStatus.ClientRegister);
            }

            services.Configure(name, configureOptions);

            services.AddHttpClient(name, (serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptionsSnapshot<HttpApiClientOptions>>()
                    .Get(name);

                if (options.BaseAddress != null)
                {
                    httpClient.BaseAddress = options.BaseAddress;
                }

                httpClient.Timeout = options.Timeout;

                foreach (var (key, value) in options.DefaultHeaders)
                {
                    if (!httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, value))
                    {
                        httpClient.DefaultRequestHeaders.Add(key, value);
                    }
                }
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsSnapshot<HttpApiClientOptions>>()
                    .Get(name);

                if (!options.EnableRetry || options.MaxRetryCount <= 0)
                {
                    return Policy.NoOpAsync<HttpResponseMessage>();
                }

                return CreateRetryPolicy(options.MaxRetryCount, options.RetryDelay);
            })
            .AddHttpMessageHandler<HttpInterceptor>();

            services.AddScoped<IHttpApiClient>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(name);

                var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsSnapshot<HttpApiClientOptions>>()
                    .Get(name);

                return new HttpApiClient(httpClient);
            });

            return services;
        }

        private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(int maxRetryCount, TimeSpan retryDelay)
        {
            return Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode &&
                    (int)response.StatusCode >= 500)
                .WaitAndRetryAsync(
                    retryCount: maxRetryCount,
                    sleepDurationProvider: retryAttempt => retryDelay * Math.Pow(2, retryAttempt - 1),
                    onRetryAsync: async (outcome, timespan, retryAttempt, context) =>
                    {
                        var message = outcome.Exception != null
                            ? $"Request failed, will retry in {timespan.TotalSeconds} seconds (attempt {retryAttempt}): {outcome.Exception.Message}"
                            : $"Request returned {outcome.Result.StatusCode}, will retry in {timespan.TotalSeconds} seconds (attempt {retryAttempt})";

                        Log4Logger.Logger.Warn(message);
                        await Task.CompletedTask;
                    }
                );
        }
    }
}
