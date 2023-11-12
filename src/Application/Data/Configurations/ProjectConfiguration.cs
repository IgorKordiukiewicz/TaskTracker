using Domain.Organizations;
using Domain.Projects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId);

        builder.HasMany(x => x.Members)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Roles)
            .WithOne()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
