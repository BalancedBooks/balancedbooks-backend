using System.Net;
using Newtonsoft.Json;

namespace BalancedBooks_API.Core.Exceptions.Models;

public class HttpVersionNotSupportedException: HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.HttpVersionNotSupported;
    public override string Message { get; } = "Http Version Not Supported";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; }
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public HttpVersionNotSupportedException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
