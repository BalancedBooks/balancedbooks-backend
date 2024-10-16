using BalancedBooksAPI.Core.Exceptions.Models;
using BalancedBooksAPI.Features.Account.GetAccount;
using Carter;
using MediatR;

namespace BalancedBooksAPI.Features.Account;

public class AccountModule : CarterModule
{
    public AccountModule(): base("/account")
    {
        WithGroupName("v1");
        WithTags("Account");
        IncludeInOpenApi();
        RequireAuthorization();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/", async (IMediator mediator) => await mediator.Send(new GetAccountQuery()))
            .WithName(nameof(GetAccountQuery))
            .Produces<INotFoundException>(404)
            .Produces<IUnauthorizedException>(401)
            .Produces<GetAccountQueryResponse>();
        
    }
}