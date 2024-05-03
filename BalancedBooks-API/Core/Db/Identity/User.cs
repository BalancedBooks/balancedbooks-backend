using BalancedBooks_API.Core.Db.Interceptors;
using Microsoft.AspNetCore.Identity;

namespace BalancedBooks_API.Core.Db.Identity;

public class User : IdentityUser<Guid>, IPublicEntity
{
    public virtual ICollection<UserLogin> Logins { get; init; }
    public virtual ICollection<UserToken> Tokens { get; init; }
    public virtual ICollection<UserRole> Roles { get; init; }

    public virtual ICollection<UserClaim> Claims { get; init; }

    public string PublicId { get; init; }
}