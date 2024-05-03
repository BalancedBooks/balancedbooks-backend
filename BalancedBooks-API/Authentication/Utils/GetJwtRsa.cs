using System.Security.Cryptography;
using BalancedBooks_API.Core.Utils;

namespace BalancedBooks_API.Authentication.Utils;

public static class AuthUtils
{
    public static (RSA publicRsa, RSA privateRsa) GetSecureRSAKeys(string publicKeyAsBase64, string privateKeyAsBase64, string privateSignKey)
    {
        var publicKey = publicKeyAsBase64.DecodeBase64();
        var privateKey = privateKeyAsBase64.DecodeBase64();

        var publicRsa = RSA.Create();
        publicRsa.ImportFromPem(publicKey);
        var privateRsa = RSA.Create();
        privateRsa.ImportFromEncryptedPem(privateKey, privateSignKey);

        return (publicRsa, privateRsa);
    }
}