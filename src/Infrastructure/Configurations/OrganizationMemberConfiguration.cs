using Domain.Organizations;
using Domain.Users;

namespace Infrastructure.Configurations;

internal class OrganizationMemberConfiguration : BaseEntityTypeConfiguration<OrganizationMember>
{
    public override void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(xx => xx.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
