using Domain.Projects;

namespace Infrastructure.Configurations;

internal class ProjectRoleConfiguration : BaseEntityTypeConfiguration<MemberRole>
{
    public override void Configure(EntityTypeBuilder<MemberRole> builder)
    {
        builder.HasKey(x => x.Id);

        base.Configure(builder);
    }
}
