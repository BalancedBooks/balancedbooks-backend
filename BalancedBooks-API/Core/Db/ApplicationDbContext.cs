using BalancedBooksAPI.Core.Db.Core;
using BalancedBooksAPI.Core.Db.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace BalancedBooksAPI.Core.Db;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions) : DbContext(contextOptions)
{
    public virtual required DbSet<User> Users { get; init; }
    public virtual required DbSet<UserSession> UserSessions { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(new SetPublicIdInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}