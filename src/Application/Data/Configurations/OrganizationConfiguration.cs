﻿using Domain.Organizations;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .IsRequired();

        builder.HasMany(x => x.Members)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Invitations)
            .WithOne()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}