using Microsoft.AspNetCore.Identity;

namespace BalancedBooksAPI.Core.Db.Identity;

public class Role : IdentityRole<Guid>
{
    public virtual ICollection<UserRole> UserRoles { get; init; }
    public virtual ICollection<RoleClaim> RoleClaims { get; init; }
}