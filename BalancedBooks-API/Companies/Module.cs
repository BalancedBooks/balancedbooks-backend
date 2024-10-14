using BalancedBooksAPI.Companies.Create;
using BalancedBooksAPI.Core.Exceptions.Models;
using MediatR;

namespace BalancedBooksAPI.Companies;

public static class CompanyModule
{
    public static void MapCompanyModuleRoutes(this WebApplication application)
    {
        application
            .MapGroup("/companies")
            .MapPost("/", async (IMediator mediator, CreateCompanyCommand cmd) => await mediator.Send(cmd))
            .WithName(nameof(CreateCompanyCommand))
            .WithGroupName("v1")
            .WithTags("Company")
            .Produces<IUnauthorizedException>(400)
            .Produces<CreateCompanyCommandResponse>()
            .RequireAuthorization()
            .WithOpenApi();
    }
}