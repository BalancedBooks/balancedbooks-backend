using Microsoft.AspNetCore.Identity;

namespace BalancedBooksAPI.Core.Db.Identity;

public class UserRole : IdentityUserRole<Guid>
{
    public virtual User User { get; init; }
};