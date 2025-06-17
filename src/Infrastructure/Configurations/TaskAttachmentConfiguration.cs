using Domain.Tasks;

namespace Infrastructure.Configurations;

internal class TaskAttachmentConfiguration : BaseEntityTypeConfiguration<TaskAttachment>
{
    public override void Configure(EntityTypeBuilder<TaskAttachment> builder)
    {
        builder.Property<int>("Key").ValueGeneratedOnAdd();
        builder.HasKey("Key");

        base.Configure(builder);
    }
}
