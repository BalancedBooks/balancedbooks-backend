using System.Security.Claims;
using BalancedBooks_API.Core.Db;
using BalancedBooks_API.Core.Db.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BalancedBooks_API.Account.GetAccount;

public record GetAccountQuery: IRequest<GetAccountQueryResponse>;

public class GetAccountQueryHandler(ApplicationDbContext dbContext, ClaimsPrincipal claimsPrincipal): IRequestHandler<GetAccountQuery, GetAccountQueryResponse>
{
    public async Task<GetAccountQueryResponse> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var f = claimsPrincipal;
        return new GetAccountQueryResponse();
    }
}

public record GetAccountQueryResponse();