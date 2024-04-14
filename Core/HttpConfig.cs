namespace balancedbooks_backend.Core;

public record HttpConfig(
    string Url
);

public static class HttpConfigExtensions
{
    public static HttpConfig AddHttpConfig(this IServiceCollection services,
        IConfigurationManager configuration)
    {
        const string httpConfigKey = "HTTP";

        services.Configure<HttpConfig>(configuration.GetSection(httpConfigKey));

        return configuration.GetSection(httpConfigKey).Get<HttpConfig>() ?? throw new Exception("Invalid HTTP Config");
    }
}