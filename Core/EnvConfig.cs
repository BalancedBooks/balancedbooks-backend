using DotNetEnv;
using DotNetEnv.Configuration;

namespace balancedbooks_backend.Core;

public static class EnvConfig
{
    public static IConfigurationRoot AddEnvironmentFlowDeps(this IConfigurationManager configurationManager)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment == null)
        {
            throw new Exception("Invalid environment");
        }

        return configurationManager
            .AddDotNetEnvMulti([".env", ".env.local", $".env.{environment.ToLower()}"], LoadOptions.TraversePath())
            .AddEnvironmentVariables()
            .Build();
    }
}