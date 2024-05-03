using Microsoft.AspNetCore.Identity;

namespace BalancedBooks_API.Core.Db.Identity;

public class UserToken : IdentityUserToken<Guid>
{
    public virtual User User { get; set; }
};