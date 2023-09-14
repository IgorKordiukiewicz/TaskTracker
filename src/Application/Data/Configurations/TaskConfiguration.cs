using Domain.Projects;
using Domain.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class TaskConfiguration : IEntityTypeConfiguration<Domain.Tasks.Task>
{
    public void Configure(EntityTypeBuilder<Domain.Tasks.Task> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ShortId);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(x => x.ProjectId);

        builder.HasOne<Domain.Tasks.TaskStatus>()
            .WithMany()
            .HasForeignKey(x => x.StatusId);
    }
}
