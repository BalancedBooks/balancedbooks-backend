using FluentValidation;
using MediatR;

namespace BalancedBooks_API.Authentication.SignUp;

public record SignUpCommand(string FirstName, string LastName, bool AgreedTerms, string Password)
    : IRequest<SignUpCommandResponse>;

public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(x => x.AgreedTerms)
            .Equal(true)
            .WithMessage("Must be true")
            .WithErrorCode("AUTH_SIGNUP_TERMS_MUST_BE_TRUE");

        RuleFor(x => x.FirstName)
            .MinimumLength(4)
            .WithMessage("Must be at least 4 char length")
            .WithErrorCode("AUTH_SIGNUP_FIRSTNAME_MIN_LENGTH")
            .MaximumLength(20)
            .WithMessage("Must be maximum 20 char length")
            .WithErrorCode("AUTH_SIGNUP_FIRSTNAME_MIN_LENGTH");

        RuleFor(x => x.LastName)
            .MinimumLength(4)
            .WithMessage("Must be at least 4 char length")
            .WithErrorCode("AUTH_SIGNUP_LASTNAME_MIN_LENGTH")
            .MaximumLength(20)
            .WithMessage("Must be maximum 20 char length")
            .WithErrorCode("AUTH_SIGNUP_LASTNAME_MAX_LENGTH");

        RuleFor(x => x.Password)
            .MinimumLength(4)
            .WithMessage("Must be at least 4 char length")
            .WithErrorCode("AUTH_SIGNUP_PASSWORD_MIN_LENGTH")
            .MaximumLength(20)
            .WithMessage("Must be maximum 20 char length")
            .WithErrorCode("AUTH_SIGNUP_PASSWORD_MAX_LENGTH");
    }
}