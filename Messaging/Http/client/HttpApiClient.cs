using Logging;
using Messaging.content;
using Messaging.Http.Exceptions;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Utils.Enumerable;
using Utils.Object;

namespace Messaging.Http
{
    public class HttpApiClient : IHttpApiClient
    {
        private readonly HttpClient _httpClient;


        public HttpApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Header

        private void AddHeaders(HttpRequestMessage request, HttpStringContent content)
        {
            if (content == null)
                return;
            if (!content.Headers.HasItem())
                return;

            try
            {
                foreach (var (key, value) in content.Headers)
                {
                    if (!request.Headers.TryAddWithoutValidation(key, value) && request.Content != null)
                    {
                        request.Content.Headers.TryAddWithoutValidation(key, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Logger.Error("HttpClient AddHeaders Error", ex);
                throw new HttpException(ex, ex.Message, HttpStatus.AddRequestHeader);
            }
        }

        #endregion Header

        #region Content

        private HttpContent BuildHttpContent(HttpStringContent content)
        {
            if (content == null)
            {
                throw new HttpException("Http request Content is null.", HttpStatus.NoContent);
            }

            var contentType = content.ContentType?.MediaType;
            if (contentType == null)
            {
                throw new HttpException("Http request ContentType is null.", HttpStatus.RequestInvalidContentType);
            }

            try
            {
                if (contentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    return content.GetJsonContent();
                }

                if (contentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                {
                    return content.GetStringContent();
                }

                if (contentType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
                {
                    return content.GetStringContent();
                }

                return JsonContent.Create(content.Content, content.ContentType, JsonEncoder.JsonOption);
            }
            catch (Exception ex)
            {
                Log4Logger.Logger.Error("HttpClient BuildHttpContent Error", ex);
                throw new HttpException(ex, ex.Message, HttpStatus.BuildRequestContent);
            }
        }

        #endregion Content

        #region Methods


        public async Task<string> GetStringAsync(string url, HttpStringContent content,
            CancellationToken? cancellationToken = null)
        {
            var cts = cancellationToken ?? CancellationToken.None;
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            AddHeaders(request, content);

            return await SendRequestAsync<string>(request, cts);
        }

        public async Task<TResponse> GetAsync<TResponse>(string url, HttpStringContent content,
            CancellationToken? cancellationToken = null) where TResponse : class
        {
            var cts = cancellationToken ?? CancellationToken.None;
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            AddHeaders(request, content);

            return await SendRequestAsync<TResponse>(request, cts);
        }

        public async Task<TResponse> PostAsync<TResponse>(string url, HttpStringContent content, 
            CancellationToken? cancellationToken = null) where TResponse : class
        {
            if (content == null)
            {
                throw new HttpException("Http request Content is null.", HttpStatus.NoContent);
            }

            var cts = cancellationToken ?? CancellationToken.None;

            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            AddHeaders(request, content);
            request.Content = BuildHttpContent(content);
            return await SendRequestAsync<TResponse>(request, cts);
        }

        private async Task<TResponse> SendRequestAsync<TResponse>(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                using var response = await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpException(
                        $"Failed to fetch http data: {response.ReasonPhrase}:{responseContent}",
                        response.StatusCode.ToHttpStatus());
                }

                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    return default!;
                }

                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonEncoder.JsonOption)
                    ?? throw new HttpException("Failed to Deserialize http response.", HttpStatus.ResponseDeserialize);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(ex);
            }
            catch (TaskCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new HttpException(ex, "Http Request Canceled.", HttpStatus.RequestCancelled);
                }
                throw new HttpException(ex, "Http Request Timeout.", HttpStatus.RequestTimeout);
            }
            catch (JsonException ex)
            {
                throw new HttpException(ex, "Failed to Deserialize http response.", HttpStatus.ResponseDeserialize);
            }
        }

        #endregion Methods
    }
}
