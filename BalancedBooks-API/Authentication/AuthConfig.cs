using System.Security.Cryptography;
using System.Text;
using BalancedBooksAPI.Authentication.Core;
using CommunityToolkit.Diagnostics;
using JWT.Algorithms;
using JWT.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
        
        var (publicRsa, privateRsa) =
            AuthenticationService.GetSecureRsaKeys(config.PublicKeyBase64, config.PrivateKeyBase64,
                config.PrivateSignKey);

        services
            .AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // TODO: cookie is not working
            .AddJwtBearer(opts =>
            { 
                var tokenValidationParameters = new TokenValidationParameters
                {
                    // Token signature will be verified using a private key.
                    
                    ValidateIssuerSigningKey = false,
                    RequireSignedTokens = false,
                    ValidateIssuer = false,
                    LogValidationExceptions = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new RsaSecurityKey(privateRsa),
                    
                    // Token will only be valid if not expired yet, with 5 minutes clock skew.
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = new TimeSpan(0, 5, 0),
                    ValidateActor = false,
                };

                opts.TokenValidationParameters = tokenValidationParameters;
                opts.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[config.CookieName];
                        return Task.CompletedTask;
                    }
                };
                

            });
        
        services.AddSingleton<IAlgorithmFactory>(new RSAlgorithmFactory(publicRsa, privateRsa));

        return services.AddAuthorization();
    }
}