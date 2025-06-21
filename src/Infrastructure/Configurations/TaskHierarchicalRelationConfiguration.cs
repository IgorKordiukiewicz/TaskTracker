using Domain.Tasks;

namespace Infrastructure.Configurations;

internal class TaskHierarchicalRelationConfiguration : BaseEntityTypeConfiguration<TaskHierarchicalRelation>
{
    public override void Configure(EntityTypeBuilder<TaskHierarchicalRelation> builder)
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
