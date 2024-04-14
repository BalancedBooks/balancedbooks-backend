using System.Reflection;
using FluentValidation;
using MediatR;

namespace balancedbooks_backend.Core.Mediatr;

public static class MediatrConfig
{
    public static void AddMediatrDeps(this IServiceCollection services)
    {
        services
            .AddMediatR(
                cfg =>
                    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)
            );

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        // TODO: temp move to validator
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}