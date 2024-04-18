using FluentValidation;

namespace BalancedBooks_Integrations_CompanyRegistry.Config;

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