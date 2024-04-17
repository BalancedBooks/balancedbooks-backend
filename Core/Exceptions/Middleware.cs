using System.Net;
using BalancedBooks_API.Core.Exceptions.Models;
using FluentValidation;
using Newtonsoft.Json;

namespace BalancedBooks_API.Core.Exceptions;

/// <summary>
/// Handles the normalization and serialization of the exceptions.
/// </summary>
public class ExceptionSerializerMiddleware(ILogger<ExceptionSerializerMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        // kestrel can throw an exception that's not originating from Exception base class
        catch (BadHttpRequestException badHttpRequestException)
        {
            await NormalizeExceptions(httpContext, badHttpRequestException);
        }
        catch (Exception exception)
        {
            await NormalizeExceptions(httpContext, exception);
        }
    }

    /// <summary>
    /// When exception happens, we format (if needed) to our own common way.
    /// Four type of exceptions can rise.
    /// 1.) ValidationException (expected)
    /// 2.) Any inheritors from HttpBaseException (expected)
    /// 3.) Exception (not expected - bug)
    /// 4.) Kestrel HttpBadRequestExceptions - e.g form upload limit (expected)
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="exception"></param>
    private async Task NormalizeExceptions(HttpContext httpContext, Exception exception)
    {
        var normalizedException = exception switch
        {
            // order is not important due to default case
            ValidationException validationException => BuildBadRequestExceptionFromFluent(
                validationException), // fluent threw them
            BadHttpRequestException badHttpRequestException
                => BuildBadRequestExceptionFromKestrel(badHttpRequestException), // kestrel threw them
            HttpBaseException httpBaseException =>
                httpBaseException, // we threw them intentionally so we can use them here (e.g NotFoundException)
            _ => HandleInternalServerErrorException(
                exception) // unhandled server exception (e.g NullReferenceException)
        };

        var traceIdentifier = GetTraceIdentifier(httpContext);

        // for debug since it helps to identify session where issue occured
        if (traceIdentifier != null)
        {
            normalizedException.TraceIdentifier = traceIdentifier;
            httpContext.Response.Headers.TryAdd("Trace-Identifier", traceIdentifier);
        }

        // setting the correct http headers from the exceptions
        httpContext.Response.StatusCode = normalizedException.IntegerHttpStatusCode;
        httpContext.Response.ContentType = "application/json";

        var serializedJsonResponse = JsonConvert.SerializeObject(normalizedException);
        await httpContext.Response.WriteAsync(serializedJsonResponse);
    }

    /// <summary>
    /// Exceptions thrown from fluent validators
    /// </summary>
    /// <param name="validationException"></param>
    /// <returns></returns>
    private BadRequestException BuildBadRequestExceptionFromFluent(ValidationException validationException)
    {
        logger.LogError(
            "[{Caller}] ValidationException from Fluent - Formatting into HttpBadRequestException",
            nameof(ExceptionSerializerMiddleware)
        );
        // convert the fluent validation error to our format
        var validationErrors = validationException.Errors
            .Select(
                // build our inner error object
                validationFailure =>
                    new ValidationError(
                        validationFailure.PropertyName.ToLower(), // the field from http body in PascalCase
                        validationFailure.ErrorCode, // coming from .WithErrorCode(...)
                        validationFailure.ErrorMessage // coming from .WithMessage(...)
                    )
            )
            .ToList();

        var badRequestHttpException = new BadRequestException(validationErrors);

        return badRequestHttpException;
    }

    /// <summary>
    /// Kestrel throws custom exceptions for non-asp // low level errors (e.g form-data upload is bigger than allowed)
    /// </summary>
    /// <param name="badHttpRequestException"></param>
    /// <returns></returns>
    private HttpBaseException BuildBadRequestExceptionFromKestrel(BadHttpRequestException badHttpRequestException)
    {
        logger.LogError(
            badHttpRequestException,
            "[{Caller}] ValidationException from Kestrel - Formatting into HttpBadRequestException",
            nameof(ExceptionSerializerMiddleware)
        );
        var statusCode = (HttpStatusCode)badHttpRequestException.StatusCode;

        return statusCode switch
        {
            HttpStatusCode.BadRequest => new BadRequestException("ROOT", "Cannot be empty"),
            // when the form upload is bigger than formOptions limits
            HttpStatusCode.RequestEntityTooLarge
                => new PayloadTooLargeException("FILE_TOO_BIG", "The file you uploaded is too big"),
            _ => new BadRequestException("UNKNOWN_ERROR", "Unknown error")
        };
    }

    /// <summary>
    /// Get operation id from request telemetry
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private static string GetTraceIdentifier(HttpContext httpContext)
    {
        return "traceIdentifier";
    }

    /// <summary>
    /// We intentionally do not leak any sensitive (stacktrace) information to the end user.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    private InternalServerErrorException HandleInternalServerErrorException(Exception exception)
    {
        logger.LogCritical(
            "[{Caller}] Unhandled Exception - Formatting into HttpInternalServerErrorException",
            nameof(ExceptionSerializerMiddleware)
        );
        logger.LogCritical(exception, "ExceptionTrace");
        var internalServerErrorException = new InternalServerErrorException();

        return internalServerErrorException;
    }
}