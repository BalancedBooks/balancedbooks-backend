using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace balancedbooks_backend.Core;

public record JwtConfig(
    string Secret,
    string Issuer,
    string Audience
);

public static class AuthConfigExtensions
{
    public static IServiceCollection AddAuthenticationDeps(this IServiceCollection services,
        IConfigurationManager configuration)
    {
        const string authConfigKey = "JWT";

        services.Configure<JwtConfig>(configuration.GetSection(authConfigKey));

        var authConfig = configuration.GetSection(authConfigKey).Get<JwtConfig>() ??
                         throw new Exception("Config is missing");
        
        var (jwtSecret, audience, issuer) = authConfig;

        services
            .AddAuthentication(opts =>
            {
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opts =>
            {
                var key = Encoding.UTF8.GetBytes(jwtSecret);

                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true
                };
                opts.SaveToken = true;
            });

        return services.AddAuthorization();
    }
}