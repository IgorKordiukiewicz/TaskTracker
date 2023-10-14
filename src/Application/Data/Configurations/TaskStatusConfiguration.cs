using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class TaskStatusConfiguration : IEntityTypeConfiguration<Domain.Tasks.TaskStatus>
{
    public void Configure(EntityTypeBuilder<Domain.Tasks.TaskStatus> builder)
    {
        builder.HasKey(x => x.Id);
    }
}