﻿using Domain.Organizations;
using Domain.Users;

namespace Infrastructure.Configurations;

internal class OrganizationConfiguration : BaseEntityTypeConfiguration<Organization>
{
    public override void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Ignore(x => x.RolesManager);

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

        builder.HasMany(x => x.Roles)
            .WithOne()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        base.Configure(builder);
    }
}
