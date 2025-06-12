using Logging;
using System.Net.Sockets;
using System.Text;
using Util;

namespace Messaging.Exceptions
{
    public class WsException : Exception
    {
        public WsErrCode ErrCode { get; private set; }

        public Dictionary<string, string> Messages { get; private set; } = new Dictionary<string, string>();

        public WsException(string message, WsErrCode errCode = WsErrCode.Exception)
            : base(message)
        {
            ErrCode = errCode;
            Messages.Add("BaseException", message);
        }

        public WsException(Exception ex, string message, WsErrCode errCode = WsErrCode.Exception)
            : base(message, ex)
        {
            ErrCode = errCode;
            if (ex.InnerException != null)
            {
                GetInnerException(ex);
            }
        }

        private void GetInnerException(Exception wsEx)
        {
            int count = 0;

            try
            {
                string? exCode = null;
                if (wsEx.InnerException == null)
                {
                    return;
                }

                if (wsEx.InnerException is HttpRequestException)
                {
                    var httpEx = wsEx.InnerException as HttpRequestException;
                    exCode = httpEx?.StatusCode?.ToString();
                }

                if (wsEx.InnerException is SocketException)
                {
                    var socketEx = wsEx.InnerException as SocketException;
                    var errCode = socketEx?.ErrorCode.NotNullString();
                    var wsErrCode = ToWsErrCode(errCode);
                    if (wsErrCode != null)
                    {
                        ErrCode = wsErrCode.Value;
                    }
                    exCode = socketEx?.SocketErrorCode.NotNullString();
                }

                var exType = wsEx.InnerException.GetType().Name;
                Messages.Add($"InnerException:[{exType}][{count++}]", 
                    $"message:{wsEx.InnerException.Message}, code={exCode}");

                GetInnerException(wsEx.InnerException);
            }
            catch (Exception ex)
            {
                Log4Logger.Logger.Error("Error while processing inner exception", ex);
            }
        }

        private WsErrCode? ToWsErrCode(string? errCode)
        {
            if (string.IsNullOrEmpty(errCode))
            {
                return WsErrCode.Exception;
            }
            if (Enum.TryParse<WsErrCode>(errCode, out var wsErrCode))
            {
                return wsErrCode;
            }
            return WsErrCode.Exception;
        }

        public string GetDetailedMessages()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Print WsException start----------");
            sb.AppendLine($"Error message summary: {Message}");
            if (Messages.HasItem())
            {
                foreach (var message in Messages)
                {
                    sb.AppendLine($"{message.Key}: {message.Value}");
                }
            }
            sb.AppendLine($"Print WsException end----------");

            return sb.ToString();
        }
    }
}
