using System.Net;
using Newtonsoft.Json;

namespace BalancedBooksAPI.Core.Exceptions.Models;

public class MethodNotAllowedException: HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.MethodNotAllowed;
    public override string Message { get; } = "Method Not Allowed";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; }
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public MethodNotAllowedException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
