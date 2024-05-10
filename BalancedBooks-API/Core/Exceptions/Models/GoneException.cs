using System.Net;
using Newtonsoft.Json;

namespace BalancedBooksAPI.Core.Exceptions.Models;

public class GoneException: HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.Gone;
    public override string Message { get; } = "Gone";
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; }
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; }

    public GoneException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
