using BalancedBooks_API.Authentication.SignInWithGoogle;
using balancedBooks_API.Authentication.SignUpWithCredentials;
using BalancedBooks_API.Core.Exceptions.Models;
using MediatR;

namespace BalancedBooks_API.Authentication;

public static class AuthenticationModule
{
    public static void MapAuthenticationModuleRoutes(this WebApplication application)
    {
        application
            .MapGroup("/auth")
            .MapPost("/sign-in/google",
                async (IMediator mediator, SignInWithGoogleCommand command) => await mediator.Send(command))
            .WithName(nameof(SignInWithGoogleCommand))
            .WithGroupName("v1")
            .WithTags("Authentication")
            .Produces<IUnauthorizedException>(400)
            .Produces<SignInWithGoogleCommandResponse>()
            .WithOpenApi();


        application.MapGroup("/auth")
            .MapPost("/sign-up/basic", async (IMediator mediator, SignUpWithCredentialsCommand command) => await mediator.Send(command))
            .WithName(nameof(SignUpWithCredentialsCommand))
            .WithGroupName("v1")
            .WithTags("Authentication")
            .Produces<IConflictException>(409)
            .Produces<SignUpWithCredentialsResponse>()
            .WithOpenApi();
    }
}