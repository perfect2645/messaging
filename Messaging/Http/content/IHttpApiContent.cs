using System.Net.Http.Headers;

namespace Messaging.Http.content
{
    public interface IHttpApiContent
    {
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, object> Content { get; set; }
        public MediaTypeHeaderValue ContentType { get; set; }
        public string RequestUrl { get; set; }
    }
}
