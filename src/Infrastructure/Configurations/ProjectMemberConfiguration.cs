using Domain.Projects;
using Domain.Users;

namespace Infrastructure.Configurations;

internal class ProjectMemberConfiguration : BaseEntityTypeConfiguration<ProjectMember>
{
    public override void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
