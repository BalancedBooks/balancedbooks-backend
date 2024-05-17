using System.Security.Claims;
using System.Security.Cryptography;
using BalancedBooksAPI.Core.Utils;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Options;

namespace BalancedBooksAPI.Authentication.Core;

public class AuthenticationService(IOptionsMonitor<AuthConfig> config)
{
    public (string passwordHash, string salt) HashPassword(string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return (passwordHash, salt);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public static (RSA publicRsa, RSA privateRsa) GetSecureRsaKeys(string publicKeyAsBase64, string privateKeyAsBase64,
        string privateSignKey)
    {
        var publicKey = publicKeyAsBase64.DecodeBase64();
        var privateKey = privateKeyAsBase64.DecodeBase64();

        var publicRsa = RSA.Create();
        publicRsa.ImportFromPem(publicKey);
        var privateRsa = RSA.Create();
        privateRsa.ImportFromEncryptedPem(privateKey, privateSignKey);

        return (publicRsa, privateRsa);
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims,
        int? expireInDays = 14)
    {
        var tokenExpireDays = config.CurrentValue.AccessTokenExpireDays;
        var expire = DateTimeOffset.UtcNow.AddHours(expireInDays ?? int.Parse(tokenExpireDays)).ToUnixTimeSeconds();

        var (publicRsa, privateRsa) = GetSecureRsaKeys(config.CurrentValue.PublicKeyBase64,
            config.CurrentValue.PrivateKeyBase64, config.CurrentValue.PrivateSignKey);

        return JwtBuilder.Create()
            .WithAlgorithm(new RS256Algorithm(publicRsa, privateRsa))
            .MustVerifySignature()
            .AddClaim("exp", expire)
            .AddClaims(claims.Select(claim => new KeyValuePair<string, object>(claim.Type, claim.Value)))
            .Encode();
    }
}