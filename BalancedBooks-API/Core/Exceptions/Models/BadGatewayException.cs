using System.Net;
using Newtonsoft.Json;

namespace BalancedBooksAPI.Core.Exceptions.Models;

public class BadGatewayException : HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.BadGateway;
    public override string Message { get; } = "Bad Gateway";

    [JsonProperty("errorCode")]
    public string ErrorCode { get; }

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public BadGatewayException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
