using balancedbooks_backend.Authentication.SignUp;
using balancedbooks_backend.Core.Exceptions.Models;
using MediatR;

namespace balancedbooks_backend.Authentication;

public static class AuthenticationRoutes
{
    public static RouteGroupBuilder MapAuthenticationRoutes(this RouteGroupBuilder group)
    {
        group
            .MapPost("/signup", async (IMediator mediator, SignUpCommand command) => await mediator.Send(command))
            .WithName(nameof(SignUpCommand))
            .WithGroupName("v1")
            .WithTags("Authentication")
            .Produces<INotFoundException>(404)
            .Produces<IUnauthorizedException>(400)
            .WithOpenApi();

        return group;
    }
}