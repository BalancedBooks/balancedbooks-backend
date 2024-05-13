namespace BalancedBooksAPI.Authentication.Claims.Google;

public struct GoogleClaims
{
    private const string BaseNameSpace = "google_";

    public static string Id => $"{BaseNameSpace}id";
}