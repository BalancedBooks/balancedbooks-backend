using BalancedBooksAPI.Core.Exceptions.Models;
using BalancedBooksAPI.PublicCompanyCertificate.Search;
using MediatR;

namespace BalancedBooksAPI.PublicCompanyCertificate;

public static class PublicCompanyCertificateModule
{
    public static void MapPublicCompanyCertificateModuleRoutes(this WebApplication app)
    {
        app
            .MapGroup("/public-company-certificate")
            .MapGet("/companies",
                async (IMediator mediator, [AsParameters] SearchCompanyQuery query) => await mediator.Send(query))
            .WithName(nameof(SearchCompanyQuery))
            .WithGroupName("v1")
            .WithTags("Public Company Certificate")
            .Produces<IUnauthorizedException>(400)
            .Produces<SearchCompanyQueryResponse>(200)
            .WithOpenApi();
    }
}