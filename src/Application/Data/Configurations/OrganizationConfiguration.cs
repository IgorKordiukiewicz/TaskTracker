using Domain.Organizations;
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

        builder.OwnsMany(x => x.Members, ownedBuilder =>
        {
            ownedBuilder.HasKey(xx => xx.Id);

            ownedBuilder.HasOne<User>()
                .WithMany()
                .HasForeignKey(xx => xx.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
