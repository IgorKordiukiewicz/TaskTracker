using Domain.Tasks;

namespace Infrastructure.Configurations;

internal class TaskRelationshipManagerConfiguration : BaseEntityTypeConfiguration<TaskRelationshipManager>
{
    public override void Configure(EntityTypeBuilder<TaskRelationshipManager> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.HierarchicalRelationships)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
