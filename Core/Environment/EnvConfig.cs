using CommunityToolkit.Diagnostics;
using DotNetEnv;
using DotNetEnv.Configuration;
using FluentValidation;

namespace BalancedBooks_API.Core.Environment;

public class AspNetEnvironmentConfig
{
    public const string ConfigKey = "ASPNETCORE";

    public required string Environment { get; init; }
}

internal class RunningEnvValidation : AbstractValidator<string>
{
    private readonly string[] _availableValues = ["development", "production"];

    public RunningEnvValidation()
    {
        RuleFor(x => x)
            .NotEmpty()
            .Must(env => _availableValues.Contains(env));
    }
}

public static class EnvConfig
{
    public static IServiceCollection AddEnvironmentFlow(this IServiceCollection service,
        IConfigurationManager configuration)
    {
        configuration
            .AddDotNetEnvMulti([".env", ".env.local"], LoadOptions.TraversePath())
            .AddEnvironmentVariables();

        service.AddOptions<AspNetEnvironmentConfig>().BindConfiguration(AspNetEnvironmentConfig.ConfigKey);

        var config = configuration.GetSection(AspNetEnvironmentConfig.ConfigKey).Get<AspNetEnvironmentConfig>();

        var validation = new RunningEnvValidation();

        Guard.IsNotNull(config);
        validation.ValidateAndThrow(config.Environment);

        return service;
    }
}