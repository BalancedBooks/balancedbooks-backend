using Microsoft.AspNetCore.Identity;

namespace BalancedBooks_API.Core.Db.Identity;

public class UserRole : IdentityUserRole<Guid>
{
    public virtual User User { get; init; }
};