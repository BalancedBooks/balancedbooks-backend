using Microsoft.AspNetCore.Identity;

namespace BalancedBooks_API.Core.Db.Identity;

public class UserLogin : IdentityUserLogin<Guid>
{
    public virtual User User { get; init; }
};