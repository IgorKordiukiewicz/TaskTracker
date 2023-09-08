using Domain.Projects;
using Domain.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.Text.Json;

namespace Application.Data.Configurations;

internal class TaskStateConfiguration : IEntityTypeConfiguration<TaskState>
{
    public void Configure(EntityTypeBuilder<TaskState> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasConversion(
            name => name.Value,
            value => new TaskStateName(value));

        builder.Property(x => x.PossibleNextStates)
            .HasConversion(
            states => JsonConvert.SerializeObject(states),
            value => JsonConvert.DeserializeObject<List<Guid>>(value));
    }
}