namespace BalancedBooksAPI.Features.Authentication.SignInWithGoogle;

/*
public record SignInWithGoogleCommand(string AccessToken) : IRequest<SignInWithGoogleCommandResponse>;

public class CommandValidator : AbstractValidator<SignInWithGoogleCommand>
{
    public CommandValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
    }
}

public class SignInWithGoogleHandler(
    ILogger<SignInWithGoogleHandler> logger,
    HttpClient httpClient,
    IOptionsMonitor<AuthConfig> authConfig,
    AuthenticationService authenticationService,
    IHttpContextAccessor accessor
)
    : IRequestHandler<SignInWithGoogleCommand, SignInWithGoogleCommandResponse>
{
    public async Task<SignInWithGoogleCommandResponse> Handle(SignInWithGoogleCommand request,
        CancellationToken cancellationToken)
    {
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

        var user = await userManager.FindByLoginAsync("google", googleToken) ??
                   await userManager.FindByEmailAsync(emailAddress);

        var claims = new List<Claim>
        {
            new(BalancedBooksCoreClaims.FirstName, givenName ?? throw exceptionForDataIssue),
            new(BalancedBooksCoreClaims.LastName, familyName ?? throw exceptionForDataIssue),
            new(GoogleClaims.Id, accountId ?? throw exceptionForDataIssue),
            new(BalancedBooksCoreClaims.EmailAddress, emailAddress ?? throw exceptionForDataIssue),
        };

        var googleIdentity = new ClaimsIdentity(claims, "google");
        var googlePrincipal = new ClaimsPrincipal(googleIdentity);

        if (user is null)
        {
            user = new User
            {
                UserName = emailAddress,
                Email = emailAddress,
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

        var accessToken =
            authenticationService.GenerateAccessToken(claims);

        accessor.HttpContext?.Response.Cookies.SetAccessTokenCookie(accessToken, authConfig.CurrentValue.CookieName,
            authConfig.CurrentValue.Domain);

        return new SignInWithGoogleCommandResponse(accessToken);
    }
}

public record SignInWithGoogleCommandResponse(string AccessToken);*/