using Domain.Organizations;
using Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class OrganizationInvitationConfiguration : BaseEntityTypeConfiguration<OrganizationInvitation>
{
    public override void Configure(EntityTypeBuilder<OrganizationInvitation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.State)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
