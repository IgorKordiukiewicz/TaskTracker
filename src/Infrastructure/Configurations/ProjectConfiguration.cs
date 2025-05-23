using Domain.Projects;

namespace Infrastructure.Configurations;

internal class ProjectConfiguration : BaseEntityTypeConfiguration<Project>
{
    public override void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Ignore(x => x.RolesManager);

        builder.HasMany(x => x.Members)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Roles)
            .WithOne()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
