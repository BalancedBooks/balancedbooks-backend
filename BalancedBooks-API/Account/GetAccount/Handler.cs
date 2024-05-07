using System.Security.Claims;
using BalancedBooks_API.Core.Db;
using BalancedBooks_API.Core.Db.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BalancedBooks_API.Account.GetAccount;

public record GetAccountQuery: IRequest<GetAccountQueryResponse>;

public class GetAccountQueryHandler(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager): IRequestHandler<GetAccountQuery, GetAccountQueryResponse>
{
    public async Task<GetAccountQueryResponse> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        ClaimsPrincipal currentUser = httpContextAccessor.HttpContext?.User;
        var user = await userManager.FindByNameAsync(currentUser.Identity.Name);

        return new GetAccountQueryResponse();
    }
}

public record GetAccountQueryResponse();