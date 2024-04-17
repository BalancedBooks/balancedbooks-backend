using BalancedBooks_API.Core.Exceptions.Models;
using BalancedBooks_API.PublicCompanyCertificate.Search;
using MediatR;

namespace BalancedBooks_API.PublicCompanyCertificate;

public static class PublicCompanyCertificateModule
{
    public static void MapPublicCompanyCertificateModuleRoutes(this WebApplication app)
    {
        app.MapGroup("/public-company-certificate")
            .MapGet("/companies",
                async (IMediator mediator, [AsParameters] SearchCompanyQuery query) => await mediator.Send(query))
            .WithName("Search Companies")
            .WithGroupName("v1")
            .WithTags("Public Company Certificate")
            .Produces<IUnauthorizedException>(400)
            .WithOpenApi();
    }
}