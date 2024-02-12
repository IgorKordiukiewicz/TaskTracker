using Domain.Tasks;
using Domain.Users;
using Infrastructure.Converters;

namespace Infrastructure.Configurations;

internal class TaskTimeLogConfiguration : BaseEntityTypeConfiguration<TaskTimeLog>
{
    public override void Configure(EntityTypeBuilder<TaskTimeLog> builder)
    {
        builder.Property<int>("Key").ValueGeneratedOnAdd();
        builder.HasKey("Key");

        builder.Property(x => x.Day)
            .HasConversion<DateOnlyConverter>()
            .HasColumnType("date");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.LoggedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        base.Configure(builder);
    }
}