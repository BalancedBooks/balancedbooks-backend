using BalancedBooksAPI.Core.Db.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalancedBooksAPI.Core.Db.Models.Company;

[EntityTypeConfiguration(typeof(CompanyEntityConfiguration))]
public class Company : IPublicEntity
{
    public Guid Id { get; }

    public required string Name { get; init; }

    public required string PublicId { get; init; }
    
    public required string CountryCode { get; init; }
    public required string TaxNumber { get; init; }
    
    public required string Domain { get; init; }

    public ICollection<CompanyMemberships> Memberships { get; } = null!;
}

public class CompanyEntityConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");

        builder.HasKey(u => u.Id);

        builder
            .HasMany<CompanyMemberships>(x => x.Memberships)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}