using Domain.Projects;
using Domain.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<Project>()
            .WithOne()
            .HasForeignKey<Workflow>(x => x.ProjectId);

        builder.HasMany(x => x.Statuses)
            .WithOne();

        builder.HasMany(x => x.Transitions)
            .WithOne();
    }
}
