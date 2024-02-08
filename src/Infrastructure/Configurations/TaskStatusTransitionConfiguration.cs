using Domain.Workflows;

namespace Infrastructure.Configurations;

internal class TaskStatusTransitionConfiguration : BaseEntityTypeConfiguration<TaskStatusTransition>
{
    public override void Configure(EntityTypeBuilder<TaskStatusTransition> builder)
    {
        builder.HasKey(x => new { x.FromStatusId, x.ToStatusId });

        base.Configure(builder);
    }
}
