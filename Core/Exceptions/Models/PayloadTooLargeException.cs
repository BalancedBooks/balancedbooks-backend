using System.Net;
using Newtonsoft.Json;

namespace balancedbooks_backend.Core.Exceptions.Models;

public class PayloadTooLargeException: HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.RequestEntityTooLarge;
    public override string Message { get; } = "Payload Too Large";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; }
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public PayloadTooLargeException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
