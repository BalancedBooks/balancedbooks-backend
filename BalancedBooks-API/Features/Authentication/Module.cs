using BalancedBooksAPI.Core.Exceptions.Models;
using BalancedBooksAPI.Features.Authentication.Logout;
using BalancedBooksAPI.Features.Authentication.SignInWithCredentials;
using BalancedBooksAPI.Features.Authentication.SignUpWithCredentials;
using Carter;
using MediatR;

namespace BalancedBooksAPI.Features.Authentication;

public class AuthenticationModule : CarterModule
{
    public AuthenticationModule() : base("/auth")
    {
        WithGroupName("v1");
        WithTags("Authentication");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app
            .MapPost("/sign-out", async (IMediator mediator) => await mediator.Send(new SignOutCommand()))
            .WithName(nameof(SignOutCommand))
            .Produces<SignOutCommandResponse>();

        app
            .MapPost("/sign-in/credentials",
                async (IMediator mediator, SignInWithCredentialsCommand command) => await mediator.Send(command))
            .WithName(nameof(SignInWithCredentialsCommand))
            .Produces<SignInWithCredentialsCommandResponse>()
            .Produces<IUnauthorizedException>(401);

        // TODO: add back when behavior is decided (keep google token alive in bg, or just one sign just to get the profile data)
        /*routeBase
            .MapPost("/sign-in/google",
                async (IMediator mediator, SignInWithGoogleCommand command) => await mediator.Send(command))
            .WithName(nameof(SignInWithGoogleCommand))
            .Produces<IUnauthorizedException>(400)
            .Produces<SignInWithGoogleCommandResponse>()
            .WithOpenApi();*/

        app
            .MapPost("/sign-up/basic",
                async (IMediator mediator, SignUpWithCredentialsCommand command) => await mediator.Send(command))
            .WithName(nameof(SignUpWithCredentialsCommand))
            .Produces<IConflictException>(409)
            .Produces<IConflictException>(409)
            .Produces<SignUpWithCredentialsResponse>();
    }
}