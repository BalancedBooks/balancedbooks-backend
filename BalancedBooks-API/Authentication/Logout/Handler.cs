using BalancedBooksAPI.Authentication.Core;
using MediatR;
using Microsoft.Extensions.Options;

namespace BalancedBooksAPI.Authentication.Logout;

public class LogoutCommand : IRequest<LogoutCommandResponse>;

public class LogoutHandler(IHttpContextAccessor accessor, IOptionsMonitor<AuthConfig> authConfig)
    : IRequestHandler<LogoutCommand, LogoutCommandResponse>
{
    public Task<LogoutCommandResponse> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var success = accessor.HttpContext?.Request.Cookies[authConfig.CurrentValue.CookieName];
        accessor.HttpContext?.Response.Cookies.DeleteCookie(authConfig.CurrentValue.Domain,
            authConfig.CurrentValue.CookieName);
        return Task.FromResult<LogoutCommandResponse>(new(success is not null));
    }
}

public class LogoutCommandResponse(bool Success);