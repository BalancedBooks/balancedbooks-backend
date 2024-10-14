using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Exceptions.Models;
using BalancedBooksAPI.Features.Authentication.Claims;
using BalancedBooksAPI.Features.Authentication.Config;
using BalancedBooksAPI.Features.Authentication.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BalancedBooksAPI.Features.Authentication.Logout;

public class SignOutCommand : IRequest<SignOutCommandResponse>;

public class SignOutHandler(
    IHttpContextAccessor accessor,
    IOptionsMonitor<AuthConfig> authConfig,
    ApplicationDbContext dbContext)
    : IRequestHandler<SignOutCommand, SignOutCommandResponse>
{
    public async Task<SignOutCommandResponse> Handle(SignOutCommand command, CancellationToken cancellationToken)
    {
        var userId = accessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == BalancedBooksCoreClaims.Id)?.Value;

        if (userId is null)
        {
            throw new UnauthorizedException("USER_NOT_FOUND", "User is absent");
        }

        var success = accessor.HttpContext?.Request.Cookies[authConfig.CurrentValue.CookieName];

        accessor.HttpContext?.Response.Cookies.SetAccessTokenCookieExpired(authConfig.CurrentValue.Domain,
            authConfig.CurrentValue.CookieName);

        var userSession =
            await dbContext.UserSessions.SingleOrDefaultAsync(x => x.UserId == Guid.Parse(userId), cancellationToken);

        if (userSession is null)
        {
            return new(success is not null);
        }

        dbContext.UserSessions.Remove(userSession);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(success is not null);
    }
}

public class SignOutCommandResponse(bool Success);