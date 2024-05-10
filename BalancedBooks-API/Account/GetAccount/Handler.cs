using BalancedBooksAPI.Core.Db;
using MediatR;

namespace BalancedBooksAPI.Account.GetAccount;

public record GetAccountQuery: IRequest<GetAccountQueryResponse>;

public class GetAccountQueryHandler(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor): IRequestHandler<GetAccountQuery, GetAccountQueryResponse>
{
    public async Task<GetAccountQueryResponse> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var currentUser = httpContextAccessor.HttpContext?.User;

        return new GetAccountQueryResponse();
    }
}

public record GetAccountQueryResponse();