using BalancedBooksAPI.Core.Exceptions.Models;
using BalancedBooksAPI.Features.Authentication.Logout;
using BalancedBooksAPI.Features.Authentication.SignInWithCredentials;
using BalancedBooksAPI.Features.Authentication.SignUpWithCredentials;
using MediatR;

namespace BalancedBooksAPI.Features.Authentication;

public static class AuthenticationModule
{
    public static void MapAuthenticationModuleRoutes(this WebApplication application)
    {
        var routeBase = application
            .MapGroup("/auth")
            .WithGroupName("v1")
            .WithTags("Authentication");

        routeBase
            .MapPost("/sign-out", async (IMediator mediator) => await mediator.Send(new SignOutCommand()))
            .WithName(nameof(SignOutCommand))
            .Produces<SignOutCommandResponse>()
            .WithOpenApi();

        routeBase
            .MapPost("/sign-in/credentials",
                async (IMediator mediator, SignInWithCredentialsCommand command) => await mediator.Send(command))
            .WithName(nameof(SignInWithCredentialsCommand))
            .Produces<SignInWithCredentialsCommandResponse>()
            .Produces<IUnauthorizedException>(401)
            .WithOpenApi();

        /*routeBase
            .MapPost("/sign-in/google",
                async (IMediator mediator, SignInWithGoogleCommand command) => await mediator.Send(command))
            .WithName(nameof(SignInWithGoogleCommand))
            .Produces<IUnauthorizedException>(400)
            .Produces<SignInWithGoogleCommandResponse>()
            .WithOpenApi();*/

        routeBase
            .MapPost("/sign-up/basic",
                async (IMediator mediator, SignUpWithCredentialsCommand command) => await mediator.Send(command))
            .WithName(nameof(SignUpWithCredentialsCommand))
            .Produces<IConflictException>(409)
            .Produces<IConflictException>(409)
            .Produces<SignUpWithCredentialsResponse>()
            .WithOpenApi();
    }
}