using System.Net;
using Newtonsoft.Json;

namespace balancedbooks_backend.Core.Exceptions.Models;

public class ConflictException : HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.Conflict;
    public override string Message { get; } = "Not Found";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; }
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public ConflictException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

}
