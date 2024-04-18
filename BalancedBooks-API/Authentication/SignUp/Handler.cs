using MediatR;

namespace BalancedBooks_API.Authentication.SignUp;

public class SignUpHandler : IRequestHandler<SignUpCommand, SignUpCommandResponse>
{
    public SignUpHandler()
    {
    }


    public async Task<SignUpCommandResponse> Handle(SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var (firstName, lastName, agreedTerms, password) = request;
        return new();
    }
}

public record SignUpCommandResponse();