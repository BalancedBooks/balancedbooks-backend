using System.Text;
using CommunityToolkit.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BalancedBooks_Integrations_CompanyRegistry;

public class CompanyRegistryConfig
{
    public const string ConfigKey = "COMPANY_REGISTRY";

    public required string Username { get; init; }
    public required string Password { get; init; }

    public required string ApiUrl { get; init; }

    public Uri ApiUrlAsUrl => new(ApiUrl);

    /* set it up during runtime */
    public string Base64Hash {
        get
        {
            var bytes = Encoding.UTF8.GetBytes($"{Username}:{Password}");
            return Convert.ToBase64String(bytes);   
        }
    }
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
    public static IServiceCollection AddCompanyRegistryIntegrationModule(this IServiceCollection services,
        CompanyRegistryConfig config)
    {
        services.AddOptions<CompanyRegistryConfig>().BindConfiguration(CompanyRegistryConfig.ConfigKey);

        var validation = new CompanyRegistryConfigValidator();

        Guard.IsNotNull(config);
        validation.ValidateAndThrow(config);

        // XML serialization requires it
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        services
            .AddTransient<CompanyRegistryHandler>()
            .AddHttpClient<CompanyRegistryHttpClient>()
            .AddHttpMessageHandler<CompanyRegistryHandler>();

        return services;
    }
}