namespace BalancedBooks_API.Core;

public class HttpConfig
{
    public const string ConfigKey = "HTTP";
    public required string Url { get; init; }
}

public static class HttpConfigExtensions
{
    public static void AddHttpConfig(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<HttpConfig>().BindConfiguration(HttpConfig.ConfigKey);
    }
}