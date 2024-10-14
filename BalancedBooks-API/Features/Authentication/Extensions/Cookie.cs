namespace BalancedBooksAPI.Features.Authentication.Extensions;

public static class AuthenticationCookieExtensions
{
    public static void AppendAccessTokenCookie(this IResponseCookies cookies, string accessToken, string cookieName,
        string domain,
        int expireDays = 14)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Domain = domain,
            Expires = DateTime.UtcNow.AddDays(expireDays)
        };

        cookies.Append(cookieName, accessToken, cookieOptions);
    }

    public static void SetAccessTokenCookieExpired(this IResponseCookies cookies, string domain, string cookieName)
    {
        cookies.Append(cookieName, "", new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Domain = domain,
            Expires = DateTime.Now.AddDays(-1)
        });
    }
}