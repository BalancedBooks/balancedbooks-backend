namespace BalancedBooksAPI.Authentication.Claims.Core;

public class BalancedBooksCoreClaims
{
    private const string BaseNameSpace = "balancedbooks_";

    public static string Id => $"{BaseNameSpace}id";

    public static string Username => $"{BaseNameSpace}username";

    public static string EmailAddress => $"{BaseNameSpace}email_address";

    public static string FirstName => $"{BaseNameSpace}first_name";

    public static string LastName => $"{BaseNameSpace}last_name";
}