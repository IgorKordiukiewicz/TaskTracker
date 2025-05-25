namespace Infrastructure.Configurations;

internal class TaskStatusConfiguration : DomainEntityTypeConfiguration<Domain.Workflows.TaskStatus>
{
    public override void Configure(EntityTypeBuilder<Domain.Workflows.TaskStatus> builder)
    {
        builder.HasKey(x => x.Id);

        base.Configure(builder);
    }
}