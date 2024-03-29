﻿using Domain.Organizations;
using Domain.Users;

namespace Infrastructure.Configurations;

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
