using Domain.Projects;
using Domain.Users;

namespace Infrastructure.Configurations;

internal class ProjectInvitationConfiguration : BaseEntityTypeConfiguration<ProjectInvitation>
{
    public override void Configure(EntityTypeBuilder<ProjectInvitation> builder)
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
