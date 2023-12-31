﻿using Domain.Organizations;
using Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

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
