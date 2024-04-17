using BalancedBooks_API.Authentication.SignUp;
using BalancedBooks_API.Core.Exceptions.Models;
using MediatR;

namespace BalancedBooks_API.Authentication;

public static class AuthenticationModule
{
    public static void MapAuthenticationModuleRoutes(this WebApplication application)
    {
        application
            .MapGroup("/auth")
            .MapPost("/signup", async (IMediator mediator, SignUpCommand command) => await mediator.Send(command))
            .WithName(nameof(SignUpCommand))
            .WithGroupName("v1")
            .WithTags("Authentication")
            .Produces<INotFoundException>(404)
            .Produces<IUnauthorizedException>(400)
            .WithOpenApi();

    }
}