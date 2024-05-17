using BalancedBooksAPI.Core.Db.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalancedBooksAPI.Core.Db.Core;

[EntityTypeConfiguration(typeof(UserEntityConfiguration))]
public class User : IPublicEntity
{
    public Guid Id { get; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required string PasswordSalt { get; init; }

    public string PublicId { get; init; } = null!;

    /* relations */
    public ICollection<UserSession> AccessTokens { get; } = null!;
}

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasKey(u => u.Id);

        builder.Property(x => x.FirstName).HasMaxLength(20);
        builder.Property(x => x.LastName).HasMaxLength(25);

        builder.Property(x => x.PublicId).HasMaxLength(30);
        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.Property(x => x.Email).HasMaxLength(30);
        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.PasswordHash).HasMaxLength(120);
        builder.HasIndex(x => x.PasswordHash).IsUnique();

        builder.Property(x => x.PasswordSalt).HasMaxLength(120);
        builder.HasIndex(x => x.PasswordSalt).IsUnique();

        /* relations */
        builder
            .HasMany<UserSession>(x => x.AccessTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}