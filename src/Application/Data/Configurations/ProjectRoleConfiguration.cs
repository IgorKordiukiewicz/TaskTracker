using Domain.Projects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class ProjectRoleConfiguration : BaseEntityTypeConfiguration<ProjectRole>
{
    public override void Configure(EntityTypeBuilder<ProjectRole> builder)
    {
        builder.HasKey(x => x.Id);

        base.Configure(builder);
    }
}
