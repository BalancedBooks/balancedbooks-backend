using System.Net;

namespace balancedbooks_backend.Core.Exceptions.Models;

/// <summary>
/// Don't throw this. This is for "unhandled exceptions"
/// </summary>
public class InternalServerErrorException : HttpBaseException
{
    public override HttpStatusCode HttpStatusCode { get; } = HttpStatusCode.InternalServerError;
    public override string Message { get; } = "Internal Server Error";
}
