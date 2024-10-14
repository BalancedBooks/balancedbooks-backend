using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalancedBooksAPI.Core.Db.Models;

[EntityTypeConfiguration(typeof(UserSessionEntityConfiguration))]
public class UserSession
{
    public required string AccessToken { get; init; }
    
    /* relations */
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
}

public class UserSessionEntityConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSession");

        builder.HasIndex(x => x.AccessToken).IsUnique();
        builder.HasKey(x => new { x.UserId, x.AccessToken });
    }
}