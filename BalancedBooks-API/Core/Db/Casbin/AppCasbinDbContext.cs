using Casbin.Persist.Adapter.EFCore;
using Microsoft.EntityFrameworkCore;

namespace BalancedBooksAPI.Core.Db.Casbin;

public class AppCasbinDbContext : CasbinDbContext<Guid>
{
    protected readonly IConfiguration _configuration;

    public AppCasbinDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AppCasbinDbContext(DbContextOptions<AppCasbinDbContext> options, IConfiguration configuration) :
        base(options)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DefaultPersistPolicyEntityTypeConfiguration<Guid>("AuthPolicy"));
    }
}