using Domain.Projects;
using Domain.Users;
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

        builder.HasOne<Domain.Workflows.TaskStatus>()
            .WithMany()
            .HasForeignKey(x => x.StatusId);

        builder.HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.AssigneeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
