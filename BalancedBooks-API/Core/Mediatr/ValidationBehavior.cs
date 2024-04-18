using FluentValidation;
using MediatR;

namespace BalancedBooks_API.Core.Mediatr;

/// <summary>
/// Runs defined validators for given mediatr handlers automatically.
/// We don't handle exceptions here
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var validatorsHooked = validators.Any();

        // we don't have validator for given handler
        if (!validatorsHooked)
        {
            logger.LogInformation("[ValidationBehavior] No validators defined");
            return await next();
        }

        var validatorsInPlace = validators.Select(validator => validator.GetType().Name);
        logger.LogInformation(
            "[ValidationBehavior] Running validators: {ValidatorsFormatted}",
            string.Join(", ", validatorsInPlace)
        );

        try
        {
            await Task.WhenAll(
                validators.Select(validator => validator.ValidateAndThrowAsync(request, cancellationToken))
            );
        }
        // we only care about validation exceptions
        catch (ValidationException validationException)
        {
            logger.LogError(
                validationException,
                "[ValidationBehavior] Failed validations: {FailedCount}(s)",
                validationException.Errors.Count()
            );
            throw;
        }

        return await next();
    }
}