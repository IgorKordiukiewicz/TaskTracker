using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class TaskStatusConfiguration : IEntityTypeConfiguration<Domain.Workflows.TaskStatus>
{
    public void Configure(EntityTypeBuilder<Domain.Workflows.TaskStatus> builder)
    {
        builder.HasKey(x => x.Id);
    }
}