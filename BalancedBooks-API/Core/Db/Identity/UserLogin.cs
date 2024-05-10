using Microsoft.AspNetCore.Identity;

namespace BalancedBooksAPI.Core.Db.Identity;

public class UserLogin : IdentityUserLogin<Guid>
{
    public virtual User User { get; init; }
};