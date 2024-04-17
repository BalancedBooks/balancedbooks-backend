using System.Text;
using CommunityToolkit.Diagnostics;
using FluentValidation;
using Flurl;

namespace BalancedBooks_API.ExternalIntegrations.CompanyRegistry;

public class CompanyRegistryConfig
{
    public const string ConfigKey = "COMPANY_REGISTRY";

    public required string Username { get; init; }
    public required string Password { get; init; }

    public required string ApiUrl { get; init; }

    public Uri ApiUrlAsUrl => new(ApiUrl);

    /* set it up during runtime */
    public required string Base64Hash { get; set; }
}

public class CompanyRegistryConfigValidator : AbstractValidator<CompanyRegistryConfig>
{
    public CompanyRegistryConfigValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.ApiUrl).Cascade(CascadeMode.Stop).NotEmpty().Must(x => x.Contains("e-cegjegyzek.hu"));
    }
}

public static class CompanyRegistryIntegrationModule
{
    public static IServiceCollection AddCompanyRegistryIntegration(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddOptions<CompanyRegistryConfig>().BindConfiguration(CompanyRegistryConfig.ConfigKey);
        var config = configuration.GetSection(CompanyRegistryConfig.ConfigKey).Get<CompanyRegistryConfig>();

        var validation = new CompanyRegistryConfigValidator();

        Guard.IsNotNull(config);
        validation.ValidateAndThrow(config);

        // XML serialization requires it
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var bytes = Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}");
        var encoded = Convert.ToBase64String(bytes);

        // set it up hash on run
        services.PostConfigure<CompanyRegistryConfig>(customOptions => { customOptions.Base64Hash = encoded; });

        services
            .AddTransient<CompanyRegistryHandler>()
            .AddHttpClient<CompanyRegistryHttpClient>()
            .AddHttpMessageHandler<CompanyRegistryHandler>();

        return services;
    }
}