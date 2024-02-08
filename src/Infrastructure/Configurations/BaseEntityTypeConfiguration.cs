namespace Infrastructure.Configurations;

internal class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property<bool>("IsDeleted")
            .HasDefaultValue(false);

        builder.HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);
    }
}
