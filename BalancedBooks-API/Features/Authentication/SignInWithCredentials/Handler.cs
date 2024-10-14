using System.Security.Claims;
using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Db.Models;
using BalancedBooksAPI.Core.Exceptions.Models;
using BalancedBooksAPI.Features.Authentication.Claims;
using BalancedBooksAPI.Features.Authentication.Extensions;
using BalancedBooksAPI.Features.Authentication.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BalancedBooksAPI.Features.Authentication.SignInWithCredentials;

public record SignInWithCredentialsCommand(string Email, string Password)
    : IRequest<SignInWithCredentialsCommandResponse>;

public class
    SignInWithCredentialsHandler(
        ApplicationDbContext dbContext,
        AuthenticationService authenticationService,
        IHttpContextAccessor accessor,
        IOptionsMonitor<AuthConfig> authConfig)
    : IRequestHandler<SignInWithCredentialsCommand, SignInWithCredentialsCommandResponse>
{
    public async Task<SignInWithCredentialsCommandResponse> Handle(SignInWithCredentialsCommand request,
        CancellationToken cancellationToken)
    {
        var (email, password) = request;

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException("INVALID_CREDENTIALS", "Invalid credentials");
        }

        var isMatch = AuthenticationService.VerifyPassword(password, user.PasswordHash);

        if (!isMatch)
        {
            throw new UnauthorizedException("INVALID_CREDENTIALS", "Invalid credentials");
        }

        var claims = new List<Claim>
        {
            new(BalancedBooksCoreClaims.Id, user.Id.ToString()),
        };

        var generatedToken = authenticationService.GenerateAccessToken(claims);

        var accessToken = new UserSession
        {
            AccessToken = generatedToken,
            UserId = user.Id,
        };

        await dbContext.UserSessions.AddAsync(accessToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        accessor.HttpContext?.Response.Cookies.AppendAccessTokenCookie(generatedToken, authConfig.CurrentValue.CookieName,
            authConfig.CurrentValue.Domain);

        return new();
    }
}

public record SignInWithCredentialsCommandResponse;