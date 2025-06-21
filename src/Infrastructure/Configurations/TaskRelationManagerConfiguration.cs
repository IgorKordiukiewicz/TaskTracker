using Domain.Tasks;

namespace Infrastructure.Configurations;

internal class TaskRelationManagerConfiguration : DomainEntityTypeConfiguration<TaskRelationManager>
{
    public override void Configure(EntityTypeBuilder<TaskRelationManager> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.HierarchicalRelations)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
