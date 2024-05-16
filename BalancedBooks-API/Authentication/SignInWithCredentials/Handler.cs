using BalancedBooksAPI.Core.Db;
using MediatR;

namespace BalancedBooksAPI.Authentication.SignInWithCredentials;

public record SignInWithCredentialsCommand : IRequest<SignInWithCredentialsCommandResponse>;

public class
    SignInWithCredentialsHandler(ApplicationDbContext dbContext)
    : IRequestHandler<SignInWithCredentialsCommand, SignInWithCredentialsCommandResponse>
{
    public async Task<SignInWithCredentialsCommandResponse> Handle(SignInWithCredentialsCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public record SignInWithCredentialsCommandResponse;