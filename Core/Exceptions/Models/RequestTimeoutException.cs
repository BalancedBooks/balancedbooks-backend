using System.Net;
using Newtonsoft.Json;

namespace balancedbooks_backend.Core.Exceptions.Models;

public class RequestTimeoutException : HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.RequestTimeout;
    public override string Message { get; } = "Request Timeout";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; }
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public RequestTimeoutException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

}
