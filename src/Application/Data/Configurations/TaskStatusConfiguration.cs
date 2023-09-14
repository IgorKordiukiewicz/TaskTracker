using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Application.Data.Configurations;

internal class TaskStatusConfiguration : IEntityTypeConfiguration<Domain.Tasks.TaskStatus>
{
    public void Configure(EntityTypeBuilder<Domain.Tasks.TaskStatus> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PossibleNextStatuses)
            .HasConversion(
            statuses => JsonConvert.SerializeObject(statuses),
            value => JsonConvert.DeserializeObject<List<Guid>>(value));
    }
}