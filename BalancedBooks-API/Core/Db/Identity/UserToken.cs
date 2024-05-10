using Microsoft.AspNetCore.Identity;

namespace BalancedBooksAPI.Core.Db.Identity;

public class UserToken : IdentityUserToken<Guid>
{
    public virtual User User { get; set; }
};