using Domain.Organizations;

namespace Infrastructure.Configurations;

internal class OrganizationRoleConfiguration : BaseEntityTypeConfiguration<OrganizationRole>
{
    public override void Configure(EntityTypeBuilder<OrganizationRole> builder)
    {
        builder.HasKey(x => x.Id);

        base.Configure(builder);
    }
}
