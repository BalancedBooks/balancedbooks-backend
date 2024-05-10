using BalancedBooksAPI.Authentication.Core;
using CommunityToolkit.Diagnostics;
using JWT.Algorithms;
using JWT.Extensions.AspNetCore;

namespace BalancedBooksAPI.Authentication;

public class AuthConfig
{
    public const string ConfigKey = "AUTH";

    [ConfigurationKeyName("DOMAIN")] public required string Domain { get; init; }
    [ConfigurationKeyName("COOKIE_NAME")] public required string CookieName { get; init; }

    [ConfigurationKeyName("JWT_PRIV_KEY_BASE64")]
    public required string PrivateKeyBase64 { get; init; }

    [ConfigurationKeyName("JWT_PRIV_SIGN_KEY")]
    public required string PrivateSignKey { get; init; }

    [ConfigurationKeyName("JWT_PUB_KEY_BASE64")]
    public required string PublicKeyBase64 { get; init; }

    [ConfigurationKeyName("JWT_EXPIRE_DAYS")]
    public required string AccessTokenExpireDays { get; init; }
}

public static class AuthConfigExtensions
{
    public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services,
        IConfigurationManager configuration)
    {
        var config = configuration.GetSection(AuthConfig.ConfigKey).Get<AuthConfig>();

        Guard.IsNotNull(config);

        services.AddOptions<AuthConfig>().BindConfiguration(AuthConfig.ConfigKey);

        services
            .AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtAuthenticationDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwt();

        var (publicRsa, privateRsa) =
            AuthenticationService.GetSecureRsaKeys(config.PublicKeyBase64, config.PrivateKeyBase64,
                config.PrivateSignKey);

        services.AddSingleton<IAlgorithmFactory>(new RSAlgorithmFactory(publicRsa, privateRsa));

        return services.AddAuthorization();
    }
}