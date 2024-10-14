using System.Net;
using Newtonsoft.Json;

namespace BalancedBooksAPI.Core.Exceptions.Models;

public interface IConflictException : IHttpBaseException
{
    string ErrorCode { get; }
    string ErrorMessage { get; }

    /// <summary>
    /// For debug purpose. Log Trace Id
    /// </summary>
    string? TraceIdentifier { get; set; }
}

public class ConflictException(string errorCode, string errorMessage) : HttpBaseException, IConflictException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.Conflict;
    public override string Message { get; } = "Not Found";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; } = errorCode;

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; } = errorMessage;
}
