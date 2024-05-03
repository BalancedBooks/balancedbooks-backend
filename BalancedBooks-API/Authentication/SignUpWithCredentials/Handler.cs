using System.Security.Claims;
using BalancedBooks_API.Authentication.Claims.Core;
using BalancedBooks_API.Core.Db;
using BalancedBooks_API.Core.Db.Identity;
using BalancedBooks_API.Core.Exceptions.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace balancedBooks_API.Authentication.SignUpWithCredentials;

public class Validator : AbstractValidator<SignUpWithCredentialsCommand>
{
    public Validator()
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

        RuleFor(x => x.EmailAddress).NotEmpty();
    }
}

public record SignUpWithCredentialsCommand(
    string FirstName,
    string LastName,
    string EmailAddress,
    bool AgreedTerms,
    string Password)
    : IRequest<SignUpWithCredentialsResponse>;

public class SignUpWithCredentialsHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ApplicationDbContext dbContext)
    : IRequestHandler<SignUpWithCredentialsCommand, SignUpWithCredentialsResponse>
{
    public async Task<SignUpWithCredentialsResponse> Handle(SignUpWithCredentialsCommand request,
        CancellationToken cancellationToken)
    {
        var (firstName, lastName, emailAddress, _, password) = request;

        var existingUser = await userManager.FindByEmailAsync(emailAddress);

        if (existingUser is not null)
        {
            throw new ConflictException("USER_ALREADY_EXISTS", "User already exists with such email");
        }

        var user = new User
        {
            UserName = emailAddress,
            Email = emailAddress,
            EmailConfirmed = false,
        };

        // TODO: send email

        await userManager.CreateAsync(user);

        var claims = new List<Claim>
        {
            new(BalancedBooksCoreClaims.Id, user.Id.ToString()),
            new(BalancedBooksCoreClaims.EmailAddress, emailAddress),
            new(BalancedBooksCoreClaims.Username, emailAddress),
            new(BalancedBooksCoreClaims.FirstName, firstName),
            new(BalancedBooksCoreClaims.LastName, lastName),
        };

        await userManager.AddClaimsAsync(user, new List<Claim>(claims));
        await userManager.AddPasswordAsync(user, password);

        await signInManager.PasswordSignInAsync(user, password, true, false);

        return new("");
    }
}

public record SignUpWithCredentialsResponse(string AccessToken);