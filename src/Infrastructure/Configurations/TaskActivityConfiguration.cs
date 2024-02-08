using Domain.Tasks;

namespace Infrastructure.Configurations;

internal class TaskActivityConfiguration : BaseEntityTypeConfiguration<TaskActivity>
{
    public override void Configure(EntityTypeBuilder<TaskActivity> builder)
    {
        builder.Property<int>("Key").ValueGeneratedOnAdd();
        builder.HasKey("Key");

        base.Configure(builder);
    }
}
