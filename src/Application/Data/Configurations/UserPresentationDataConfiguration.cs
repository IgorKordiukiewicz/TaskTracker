using Application.Data.Models;
using Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

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
