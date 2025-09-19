
using Messaging.content;

namespace Messaging.Http
{
    public interface IHttpApiClient
    {
        Task<string> GetStringAsync(string url, HttpStringContent content,
            CancellationToken? cancellationToken = null);
        Task<TResponse> GetAsync<TResponse>(string url, HttpStringContent content,
            CancellationToken? cancellationToken = null) where TResponse : class;
        Task<TResponse> PostAsync<TResponse>(string url, HttpStringContent content,
            CancellationToken? cancellationToken = null) where TResponse : class;
    }
}