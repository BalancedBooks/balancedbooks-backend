using System.Net;
using Newtonsoft.Json;

namespace BalancedBooksAPI.Core.Exceptions.Models;

public class UnprocessableEntityException: HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.UnprocessableEntity;
    public override string Message { get; } = "Unprocessable Entity";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; }
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public UnprocessableEntityException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
    
}
