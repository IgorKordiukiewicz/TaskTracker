using Domain.Notifications;
using Domain.Users;

namespace Infrastructure.Configurations;

internal class NotificationConfiguration : DomainEntityTypeConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
