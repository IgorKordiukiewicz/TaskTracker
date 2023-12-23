using Domain.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class TaskActivityConfiguration : BaseEntityTypeConfiguration<TaskActivity>
{
    public override void Configure(EntityTypeBuilder<TaskActivity> builder)
    {
        builder.Property<int>("Key").ValueGeneratedOnAdd();
        builder.HasKey("Key");

        base.Configure(builder);
    }
}
