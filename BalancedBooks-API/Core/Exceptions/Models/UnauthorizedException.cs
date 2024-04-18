using System.Net;
using Newtonsoft.Json;

namespace BalancedBooks_API.Core.Exceptions.Models;

public interface IUnauthorizedException: IHttpBaseException
{
    string ErrorCode { get; }
    string ErrorMessage { get; }

    /// <summary>
    /// For debug purpose. Log Trace Id
    /// </summary>
    string? TraceIdentifier { get; set; }
}

public class UnauthorizedException(string errorCode, string errorMessage) : HttpBaseException, IUnauthorizedException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.Unauthorized;
    public override string Message { get; } = "Unauthorized";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; } = errorCode;

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; } = errorMessage;
}
