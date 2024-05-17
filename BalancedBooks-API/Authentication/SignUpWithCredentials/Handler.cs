using System.Security.Claims;
using BalancedBooksAPI.Authentication.Claims.Core;
using BalancedBooksAPI.Authentication.Core;
using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Db.Core;
using BalancedBooksAPI.Core.Exceptions.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BalancedBooksAPI.Authentication.SignUpWithCredentials;

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
    AuthenticationService authenticationService,
    ApplicationDbContext dbContext,
    IOptionsMonitor<AuthConfig> authConfig,
    IHttpContextAccessor accessor)
    : IRequestHandler<SignUpWithCredentialsCommand, SignUpWithCredentialsResponse>
{
    public async Task<SignUpWithCredentialsResponse> Handle(SignUpWithCredentialsCommand request,
        CancellationToken cancellationToken)
    {
        var (firstName, lastName, emailAddress, _, password) = request;

        var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == emailAddress, cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictException("USER_ALREADY_EXISTS", "User already exists with such email");
        }

        var (passwordHash, salt) = authenticationService.HashPassword(password);

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = emailAddress,
            PasswordSalt = salt,
            PasswordHash = passwordHash
        };

        // TODO: send email

        await dbContext.Users.AddAsync(user, cancellationToken);

        var claims = new List<Claim>
        {
            new(BalancedBooksCoreClaims.Id, user.Id.ToString()),
        };

        var generatedToken =
            authenticationService.GenerateAccessToken(claims);
        
        var userSession = new UserSession
        {
            AccessToken = generatedToken,
            UserId = user.Id
        };

        await dbContext.UserSessions.AddAsync(userSession, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);

        accessor.HttpContext?.Response.Cookies.SetAccessTokenCookie(generatedToken, authConfig.CurrentValue.CookieName,
            authConfig.CurrentValue.Domain);

        return new(generatedToken);
    }
}

public record SignUpWithCredentialsResponse(string AccessToken);