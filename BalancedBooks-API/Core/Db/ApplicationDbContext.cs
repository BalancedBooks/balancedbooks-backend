using BalancedBooksAPI.Core.Db.Interceptors;
using BalancedBooksAPI.Core.Db.Models;
using BalancedBooksAPI.Core.Db.Models.Company;
using Microsoft.EntityFrameworkCore;

namespace BalancedBooksAPI.Core.Db;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions) : DbContext(contextOptions)
{
    public virtual required DbSet<User> Users { get; init; }
    public virtual required DbSet<UserSession> UserSessions { get; init; }

    public virtual required DbSet<Company> Companies { get; init; }
    public virtual required DbSet<CompanyMemberships> CompanyMemberships { get; init; }

    private readonly IHostEnvironment Environment;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHostEnvironment env)
        : this(options)
    {
        Environment = env;
    }

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