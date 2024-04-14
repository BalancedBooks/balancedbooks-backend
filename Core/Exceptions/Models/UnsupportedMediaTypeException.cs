using System.Net;
using Newtonsoft.Json;

namespace balancedbooks_backend.Core.Exceptions.Models;

public class UnsupportedMediaTypeException: HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.UnsupportedMediaType;
    public override string Message { get; } = "Unsupported Media Type";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; }
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public UnsupportedMediaTypeException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
