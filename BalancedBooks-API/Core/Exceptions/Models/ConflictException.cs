﻿using System.Net;
using Newtonsoft.Json;

namespace BalancedBooks_API.Core.Exceptions.Models;

public interface IConflictException : IHttpBaseException
{
    string ErrorCode { get; }
    string ErrorMessage { get; }

    /// <summary>
    /// For debug purpose. Log Trace Id
    /// </summary>
    string? TraceIdentifier { get; set; }
}

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
