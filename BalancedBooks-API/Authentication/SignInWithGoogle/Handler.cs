using System.Net.Http.Headers;
using System.Security.Claims;
using balancedBooks_API.Authentication;
using BalancedBooks_API.Authentication.Claims.Google;
using BalancedBooks_API.Authentication.Utils;
using BalancedBooks_API.Core.Db.Identity;
using BalancedBooks_API.Core.Exceptions.Models;
using FluentValidation;
using Flurl;
using Google.Apis.PeopleService.v1.Data;
using JWT.Algorithms;
using JWT.Builder;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BalancedBooks_API.Authentication.SignInWithGoogle;

public record SignInWithGoogleCommand(string AccessToken) : IRequest<SignInWithGoogleCommandResponse>;

public class CommandValidator : AbstractValidator<SignInWithGoogleCommand>
{
    public CommandValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
    }
}

public class SignInWithGoogleHandler(
    IOptions<AuthConfig> authConfig,
    ILogger<SignInWithGoogleHandler> logger,
    UserManager<User> userManager,
    HttpClient httpClient,
    SignInManager<User> signInManager,
    IHttpContextAccessor accessor
)
    : IRequestHandler<SignInWithGoogleCommand, SignInWithGoogleCommandResponse>
{
    public async Task<SignInWithGoogleCommandResponse> Handle(SignInWithGoogleCommand request,
        CancellationToken cancellationToken)
    {
        var config = authConfig.Value;
        var googleToken = request.AccessToken;

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", googleToken);

        var url = "https://people.googleapis.com/v1/people/me".AppendQueryParam(
            "personFields=emailAddresses,locales,names");

        var userInfo = await httpClient.GetFromJsonAsync<Person>(
            url, cancellationToken);

        if (userInfo is null)
        {
            logger.LogError("[AUTH] - Google AccessToken Error. {Token}", request.AccessToken);
            throw new UnauthorizedException("INVALID_GOOGLE_TOKEN", "Invalid access token for Google");
        }

        var accountId = userInfo.ResourceName.Split("/").LastOrDefault();
        var givenName = userInfo.Names.FirstOrDefault()?.GivenName;
        var familyName = userInfo.Names.FirstOrDefault()?.FamilyName;
        var emailAddressMeta = userInfo.EmailAddresses.FirstOrDefault(x => x.Metadata.SourcePrimary == true);
        var emailAddress = emailAddressMeta?.Value;

        var exceptionForDataIssue = new UnauthorizedException("GOOGLE_INVALID_ACCOUNT_STATE",
            "We could not get some information for user account");

        if (emailAddress is null)
        {
            throw exceptionForDataIssue;
        }

        var user = await userManager.FindByLoginAsync("google", googleToken) ?? await userManager.FindByEmailAsync(emailAddress);

        var claims = new List<Claim>
        {
            new(GoogleClaims.FirstName, givenName ?? throw exceptionForDataIssue),
            new(GoogleClaims.LastName, familyName ?? throw exceptionForDataIssue),
            new(GoogleClaims.Id, accountId ?? throw exceptionForDataIssue),
            new(GoogleClaims.EmailAddress, emailAddress),
            new(GoogleClaims.EmailAddressVerified,
                emailAddressMeta?.Metadata.Verified.ToString() ?? throw exceptionForDataIssue)
        };

        var googleIdentity = new ClaimsIdentity(claims, "google");
        var googlePrincipal = new ClaimsPrincipal(googleIdentity);

        if (user is null)
        {
            user = new User
            {
                UserName = emailAddressMeta.Value,
                Email = emailAddressMeta.Value,
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(user);
        }
        else
        {
            await userManager.RemoveClaimsAsync(user, claims);
        }


        await userManager.AddClaimsAsync(user, new List<Claim>(claims));
        await userManager.AddLoginAsync(user,
            new ExternalLoginInfo(googlePrincipal, "google", accountId, "Google"));

        var (publicRsa, privateRsa) =
            AuthUtils.GetSecureRSAKeys(config.PublicKeyBase64, config.PrivateKeyBase64, config.PrivateSignKey);

        var expire = DateTimeOffset.UtcNow.AddHours(int.Parse(config.AccessTokenExpireDays)).ToUnixTimeSeconds();

        var accessToken = JwtBuilder.Create()
            .WithAlgorithm(new RS256Algorithm(publicRsa, privateRsa))
            .MustVerifySignature()
            .AddClaim("userId", user.Id)
            .AddClaim("exp", expire)
            .Encode();

        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Domain = "balancedbooks.dev",
            Expires = DateTime.UtcNow.AddDays(14)
        };

        accessor.HttpContext?.Response.Cookies.Append("balancedbooks_auth_session", accessToken, cookieOptions);

        return new SignInWithGoogleCommandResponse(accessToken);
    }
}

public record SignInWithGoogleCommandResponse(string AccessToken);