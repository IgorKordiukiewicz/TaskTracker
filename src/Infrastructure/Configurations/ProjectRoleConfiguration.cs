using Domain.Projects;

namespace Infrastructure.Configurations;

internal class ProjectRoleConfiguration : BaseEntityTypeConfiguration<ProjectRole>
{
    public override void Configure(EntityTypeBuilder<ProjectRole> builder)
    {
        builder.HasKey(x => x.Id);

        base.Configure(builder);
    }
}
