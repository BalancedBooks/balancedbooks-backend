using BalancedBooksAPI.Core.Db;
using BalancedBooksAPI.Core.Exceptions.Models;
using BalancedBooksAPI.Features.Authentication;
using BalancedBooksAPI.Features.Authentication.Claims;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BalancedBooksAPI.Features.Companies.Create;

public record CreateCompanyCommand(string Name, string VatId, string CurrencyCode)
    : IRequest<CreateCompanyCommandResponse>;

public class CreateCompanyCommandHandler(
    ApplicationDbContext dbContext,
    IHttpContextAccessor accessor,
    IOptionsMonitor<AuthConfig> authConfig)
    : IRequestHandler<CreateCompanyCommand, CreateCompanyCommandResponse>
{
    public async Task<CreateCompanyCommandResponse> Handle(CreateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = accessor.HttpContext?.User;

        var userId = currentUser?.Claims.FirstOrDefault(x => x.Type == BalancedBooksCoreClaims.Id)?.Value;

        if (userId is null)
        {
            throw new UnauthorizedException("USER_NOT_FOUND", "");
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException("USER_NOT_FOUND", "");
        }
        
        

        return new CreateCompanyCommandResponse(user.Email, user.FirstName, user.LastName);
    }
}

public record CreateCompanyCommandResponse(string Email, string FirstName, string LastName);