using System.Reflection;
using FluentValidation;
using MediatR;

namespace BalancedBooksAPI.Core.Mediatr;

public static class MediatrConfig
{
    public static IServiceCollection AddMediatrHandlers(this IServiceCollection services)
    {
        services
            .AddMediatR(
                cfg =>
                    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)
            );

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // TODO: temp move to validator

        return services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}