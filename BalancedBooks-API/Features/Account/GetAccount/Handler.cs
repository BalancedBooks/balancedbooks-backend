using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Exceptions.Models;
using BalancedBooksAPI.Features.Authentication.Claims;
using BalancedBooksAPI.Features.Authentication.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BalancedBooksAPI.Features.Account.GetAccount;

public record GetAccountQuery : IRequest<GetAccountQueryResponse>;

public class GetAccountQueryHandler(
    ApplicationDbContext dbContext,
    IHttpContextAccessor accessor,
    IOptionsMonitor<AuthConfig> authConfig)
    : IRequestHandler<GetAccountQuery, GetAccountQueryResponse>
{
    public async Task<GetAccountQueryResponse> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var currentUser = accessor.HttpContext?.User;

        var userId = currentUser?.Claims.FirstOrDefault(x => x.Type == BalancedBooksCoreClaims.Id)?.Value;

        if (userId is null)
        {
            throw new UnauthorizedException("USER_NOT_FOUND", "");
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException("USER_NOT_FOUND", "");
        }

        return new GetAccountQueryResponse(user.Email, user.FirstName, user.LastName);
    }
}

public record GetAccountQueryResponse(string Email, string FirstName, string LastName);