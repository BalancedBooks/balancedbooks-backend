using System.Net;
using Newtonsoft.Json;

namespace BalancedBooks_API.Core.Exceptions.Models;

public class ServiceUnavailableException: HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.ServiceUnavailable;
    public override string Message { get; } = "Service Unavailable";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; }
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public ServiceUnavailableException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
