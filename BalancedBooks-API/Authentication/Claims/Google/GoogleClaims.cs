namespace BalancedBooks_API.Authentication.Claims.Google;

public struct GoogleClaims
{
    private const string BaseNameSpace = "google_";

    public static string Id => $"{BaseNameSpace}id";

    public static string Username => $"{BaseNameSpace}username";

    public static string EmailAddress => $"{BaseNameSpace}emailAddress";
    
    public static string EmailAddressVerified => $"{BaseNameSpace}emailAddressVerified";

    public static string FirstName => $"{BaseNameSpace}firstName";

    public static string LastName => $"{BaseNameSpace}lastName";
}