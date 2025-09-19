# Messaging

Provides below clients:
- WebsocketClient
- HttpClient
- RabbitMqClient


## Microsoft.Net.Http.Headers
``` Csharp
    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // 全局设置默认请求头（如认证令牌）
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", "your-jwt-token");
    }

    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/users");
        
    // 构建 Accept 头：application/json（优先级 1.0），application/xml（优先级 0.8）
    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.8));
```
## Microsoft.Extensions.Http.Polly
``` Csharp
    services.AddHttpClient<ApiClient>()
        .AddPolicyHandler(GetRetryPolicy());
    
    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        // 定义重试策略：遇到请求失败时，重试 3 次，间隔时间逐渐增加
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
```