using System.Net;
using Newtonsoft.Json;

namespace BalancedBooksAPI.Core.Exceptions.Models;

public class ForbiddenException(string errorCode, string errorMessage) : HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.Forbidden;
    public override string Message { get; } = "Forbidden";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; } = errorCode;

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; } = errorMessage;
}
