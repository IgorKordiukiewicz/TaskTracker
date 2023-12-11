using Domain.Organizations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class OrganizationRoleConfiguration : BaseEntityTypeConfiguration<OrganizationRole>
{
    public override void Configure(EntityTypeBuilder<OrganizationRole> builder)
    {
        builder.HasKey(x => x.Id);

        base.Configure(builder);
    }
}
