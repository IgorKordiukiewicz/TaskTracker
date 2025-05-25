using Domain.Tasks;
using Domain.Users;

namespace Infrastructure.Configurations;

internal class TaskCommentConfiguration : DomainEntityTypeConfiguration<TaskComment>
{
    public override void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
