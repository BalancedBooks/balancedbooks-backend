using Microsoft.AspNetCore.Identity;

namespace BalancedBooks_API.Core.Db.Identity;

public class UserClaim : IdentityUserClaim<Guid>
{
    public virtual User User { get; init; }
}