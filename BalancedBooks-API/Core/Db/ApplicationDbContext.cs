using BalancedBooks_API.Core.Db.Identity;
using BalancedBooks_API.Core.Db.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BalancedBooks_API.Core.Db;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim,
        UserToken>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(new SetPublicIdInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(e =>
        {
            e.ToTable(name: "User");

            /* indexes/keys */
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
            e.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");

            /* relations */
            e.HasMany(u => u.Claims)
                .WithOne(uc => uc.User).HasForeignKey(uc => uc.UserId).IsRequired();
            e.HasMany(u => u.Logins)
                .WithOne(ul => ul.User).HasForeignKey(ul => ul.UserId).IsRequired();
            e.HasMany(u => u.Tokens)
                .WithOne(ut => ut.User).HasForeignKey(ut => ut.UserId).IsRequired();
            e.HasMany(u => u.Roles)
                .WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId).IsRequired();
        });

        builder.Entity<Role>(entity =>
        {
            entity.ToTable(name: "Role");

            /* indexes/keys */
            entity.HasKey(r => r.Id);
            entity.HasIndex(e => e.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

            /* columns */
            entity.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

            /* relations */
            entity.HasMany(r => r.UserRoles).WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
            entity.HasMany(r => r.RoleClaims).WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
        });

        builder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");

            entity.HasKey(key => new { key.UserId, key.RoleId });
        });

        builder.Entity<UserClaim>(entity =>
        {
            entity.ToTable("UserClaim");

            entity.HasKey(uc => uc.Id);
        });

        builder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(key => new { key.UserId, key.ProviderKey, key.LoginProvider });

            entity.ToTable("UserLogin");
        });

        builder.Entity<RoleClaim>(entity => { entity.ToTable("RoleClaim"); });

        builder.Entity<UserToken>(entity =>
        {
            entity.ToTable("UserToken");

            entity.HasKey(key => new { key.UserId, key.LoginProvider, key.Name });
        });
    }
}