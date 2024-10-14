using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalancedBooksAPI.Core.Db.Models.Company;

[EntityTypeConfiguration(typeof(CompanyMemberEntityConfiguration))]
public class CompanyMemberships
{
    public Guid Id { get; }
    public required string Name { get; init; }

    public required Guid CompanyId { get; init; }
    public required Company Company { get; init; }

    public required Guid UserId { get; init; }
    public required User User { get; init; }
}

public class CompanyMemberEntityConfiguration : IEntityTypeConfiguration<CompanyMemberships>
{
    public void Configure(EntityTypeBuilder<CompanyMemberships> builder)
    {
        builder.ToTable("CompanyMemberships");

        builder.HasKey(u => u.Id);
    }
}