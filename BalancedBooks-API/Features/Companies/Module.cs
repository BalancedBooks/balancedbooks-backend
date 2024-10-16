using BalancedBooksAPI.Core.Exceptions.Models;
using BalancedBooksAPI.Features.Companies.Create;
using Carter;
using MediatR;

namespace BalancedBooksAPI.Features.Companies;

public class CompanyModule: CarterModule
{
    public CompanyModule(): base("/companies")
    {
        WithGroupName("v1");
        WithTags("Company");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app
            .MapPost("/", async (IMediator mediator, CreateCompanyCommand cmd) => await mediator.Send(cmd))
            .WithName(nameof(CreateCompanyCommand))
            .Produces<IUnauthorizedException>(401)
            .Produces<CreateCompanyCommandResponse>()
            .RequireAuthorization();
    }
}