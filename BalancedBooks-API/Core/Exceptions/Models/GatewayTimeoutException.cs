using System.Net;
using Newtonsoft.Json;

namespace BalancedBooksAPI.Core.Exceptions.Models;

public class GatewayTimeoutException(string errorCode, string errorMessage) : HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.GatewayTimeout;
    public override string Message { get; } = "Gateway Timeout";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; } = errorCode;

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; } = errorMessage;
}
