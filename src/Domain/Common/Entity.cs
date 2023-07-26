namespace Domain.Common;

public abstract class Entity<TId>
{
    public TId Id { get; private init; }

    protected Entity(TId id)
    {
        Id = id;
    }
}
