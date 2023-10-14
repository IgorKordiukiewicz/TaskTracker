using Domain.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class TaskStatusTransitionConfiguration : IEntityTypeConfiguration<Domain.Tasks.TaskStatusTransition>
{
    public void Configure(EntityTypeBuilder<TaskStatusTransition> builder)
    {
        builder.HasKey(x => new { x.FromStatusId, x.ToStatusId });
    }
}
