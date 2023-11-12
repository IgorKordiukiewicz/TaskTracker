using Domain.Projects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class ProjectRoleConfiguration : IEntityTypeConfiguration<ProjectRole>
{
    public void Configure(EntityTypeBuilder<ProjectRole> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
