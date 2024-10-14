using System.Net;
using Newtonsoft.Json;

namespace BalancedBooksAPI.Core.Exceptions.Models;

public interface IBadRequestException : IHttpBaseException
{
    string ErrorCode { get; }
    string ErrorMessage { get; }

    /// <summary>
    /// For debug purpose. Log Trace Id
    /// </summary>
    string? TraceIdentifier { get; set; }
    
    public List<ValidationError> ValidationErrors { get; set; }
}


public record ValidationError(string? FieldName, string Code, string Message)
{
    /// <summary>
    /// Only valid for form validation type of errors 
    /// </summary>
    [JsonProperty("fieldName")]
    public string? FieldName { get; set; } = FieldName;
    [JsonProperty("code")]
    public string Code { get; set; } = Code;
    [JsonProperty("message")]
    public string Message { get; set; } = Message;
}

public class BadRequestException : HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.BadRequest;
    public override string Message => "Bad Request";

    [JsonProperty("validationErrors")]
    public List<ValidationError> ValidationErrors { get; set; }
    
    [JsonConstructor]
#pragma warning disable CS8618
    public BadRequestException() { }
#pragma warning restore CS8618

    public BadRequestException(List<ValidationError> validationErrors) => ValidationErrors = validationErrors;

    public BadRequestException(ValidationError validationError) => ValidationErrors = new List<ValidationError> {validationError};

    public BadRequestException(string code, string message, string? fieldName = null) => ValidationErrors = new List<ValidationError> {new(fieldName, code, message)};
}
