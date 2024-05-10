using Microsoft.AspNetCore.Identity;

namespace BalancedBooksAPI.Core.Db.Identity;

public class UserClaim : IdentityUserClaim<Guid>
{
    public virtual User User { get; init; }
}