using System.Net;
using Newtonsoft.Json;

namespace balancedbooks_backend.Core.Exceptions.Models;

public interface INotFoundException : IHttpBaseException
{
    string ErrorCode { get; }
    string ErrorMessage { get; }

    /// <summary>
    /// For debug purpose. Log Trace Id
    /// </summary>
    string? TraceIdentifier { get; set; }
}

public class NotFoundException(string errorCode, string errorMessage) : HttpBaseException, INotFoundException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.NotFound;
    public override string Message { get; } = "Not Found";

    [JsonProperty("errorCode")] public string ErrorCode { get; } = errorCode;

    [JsonProperty("errorMessage")] public string ErrorMessage { get; } = errorMessage;
}