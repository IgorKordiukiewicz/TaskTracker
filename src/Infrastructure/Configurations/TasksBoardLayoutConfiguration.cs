using Infrastructure.Converters;
using Infrastructure.Models;

namespace Infrastructure.Configurations;

internal class TasksBoardLayoutConfiguration : BaseEntityTypeConfiguration<TasksBoardLayout>
{
    public override void Configure(EntityTypeBuilder<TasksBoardLayout> builder)
    {
        builder.HasKey(x => x.ProjectId);

        builder.Property(x => x.Columns)
            .HasConversion<JsonConverter<TasksBoardColumn[]>>();

        base.Configure(builder);
    }
}
