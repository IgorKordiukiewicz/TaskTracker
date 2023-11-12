using Domain.Organizations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class OrganizationRoleConfiguration : IEntityTypeConfiguration<OrganizationRole>
{
    public void Configure(EntityTypeBuilder<OrganizationRole> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
