using System.Net;
using Newtonsoft.Json;

namespace BalancedBooksAPI.Core.Exceptions.Models;

public class BadGatewayException(string errorCode, string errorMessage) : HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.BadGateway;
    public override string Message { get; } = "Bad Gateway";

    [JsonProperty("errorCode")]
    public string ErrorCode { get; } = errorCode;

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; } = errorMessage;
}
