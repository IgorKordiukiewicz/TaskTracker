using Domain.Tasks;

namespace Infrastructure.Configurations;

internal class TaskHierarchicalRelationshipConfiguration : BaseEntityTypeConfiguration<TaskHierarchicalRelationship>
{
    public override void Configure(EntityTypeBuilder<TaskHierarchicalRelationship> builder)
    {
        builder.HasKey(x => new { x.ParentId, x.ChildId });

        builder.HasOne<Domain.Tasks.Task>()
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Tasks.Task>()
            .WithMany()
            .HasForeignKey(x => x.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
