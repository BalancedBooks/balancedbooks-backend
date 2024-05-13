using BalancedBooksAPI.Authentication;
using BalancedBooksAPI.Authentication.Claims.Core;
using BalancedBooksAPI.Authentication.Core;
using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Exceptions.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BalancedBooksAPI.Account.GetAccount;

public record GetAccountQuery : IRequest<GetAccountQueryResponse>;

public class GetAccountQueryHandler(
    ApplicationDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IHttpContextAccessor accessor,
    IOptionsMonitor<AuthConfig> authConfig)
    : IRequestHandler<GetAccountQuery, GetAccountQueryResponse>
{
    public async Task<GetAccountQueryResponse> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var currentUser = httpContextAccessor.HttpContext?.User;

        var userId = currentUser?.Claims.FirstOrDefault(x => x.Type == BalancedBooksCoreClaims.Id)?.Value;

        if (userId is null)
        {
            throw new UnauthorizedException("USER_NOT_FOUND", "");
        }

        var user = await dbContext.Users.Include(x => x.Claims)
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException("USER_NOT_FOUND", "");
        }

        var claims = user.Claims;

        var email = claims.FirstOrDefault(x => x.ClaimType == BalancedBooksCoreClaims.EmailAddress)?.ClaimValue ?? "";
        var firstName = claims.FirstOrDefault(x => x.ClaimType == BalancedBooksCoreClaims.FirstName)?.ClaimValue ?? "";
        var lastName = claims.FirstOrDefault(x => x.ClaimType == BalancedBooksCoreClaims.LastName)?.ClaimValue ?? "";

        return new GetAccountQueryResponse(email, firstName, lastName);
    }
}

public record GetAccountQueryResponse(string Email, string FirstName, string LastName);