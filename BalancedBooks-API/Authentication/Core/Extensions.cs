namespace BalancedBooksAPI.Authentication.Core;

public static class AuthUtils
{
    public static void SetAccessTokenCookie(this IResponseCookies cookies, string accessToken, string cookieName,
        string domain,
        int expireDays = 14)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Domain = domain,
            Expires = DateTime.UtcNow.AddDays(expireDays)
        };

        cookies.Append(cookieName, accessToken, cookieOptions);
    }
}