using BalancedBooksAPI.Account.GetAccount;
using BalancedBooksAPI.Core.Exceptions.Models;
using MediatR;

namespace BalancedBooksAPI.Account;

public static class AccountModule
{
    public static void MapAccountModuleRoutes(this WebApplication application)
    {
        application
            .MapGroup("/account")
            .MapGet("/", async (IMediator mediator) => await mediator.Send(new GetAccountQuery()))
            .WithName(nameof(GetAccountQuery))
            .WithGroupName("v1")
            .WithTags("Account")
            .Produces<INotFoundException>(404)
            .Produces<IUnauthorizedException>(400)
            .Produces<GetAccountQueryResponse>(200)
            .RequireAuthorization()
            .WithOpenApi();
    }
}