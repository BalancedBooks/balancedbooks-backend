using BalancedBooksAPI.Authentication.Logout;
using BalancedBooksAPI.Authentication.SignInWithGoogle;
using BalancedBooksAPI.Authentication.SignUpWithCredentials;
using BalancedBooksAPI.Core.Exceptions.Models;
using MediatR;

namespace BalancedBooksAPI.Authentication;

public static class AuthenticationModule
{
    public static void MapAuthenticationModuleRoutes(this WebApplication application)
    {
        var routeBase = application
            .MapGroup("/auth")
            .WithGroupName("v1")
            .WithTags("Authentication");

        routeBase
            .MapPost("/sign-out", async (IMediator mediator) => await mediator.Send(new LogoutCommand()))
            .WithName(nameof(LogoutCommand))
            .Produces<LogoutCommandResponse>()
            .WithOpenApi();

        routeBase
            .MapPost("/sign-in/google",
                async (IMediator mediator, SignInWithGoogleCommand command) => await mediator.Send(command))
            .WithName(nameof(SignInWithGoogleCommand))
            .Produces<IUnauthorizedException>(400)
            .Produces<SignInWithGoogleCommandResponse>()
            .WithOpenApi();

        routeBase
            .MapPost("/sign-up/basic",
                async (IMediator mediator, SignUpWithCredentialsCommand command) => await mediator.Send(command))
            .WithName(nameof(SignUpWithCredentialsCommand))
            .Produces<IConflictException>(409)
            .Produces<SignUpWithCredentialsResponse>()
            .WithOpenApi();
    }
}