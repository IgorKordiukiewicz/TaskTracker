using Domain.Users;
using Infrastructure.Models;

namespace Infrastructure.Configurations;

internal class UserPresentationDataConfiguration : BaseEntityTypeConfiguration<UserPresentationData>
{
    public override void Configure(EntityTypeBuilder<UserPresentationData> builder)
    {
        builder.HasKey(x => x.UserId);

        builder.Property(x => x.AvatarColor)
            .HasMaxLength(7);

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<UserPresentationData>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
