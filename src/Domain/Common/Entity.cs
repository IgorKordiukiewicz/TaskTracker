namespace Domain.Common;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected init; }

    private readonly List<DomainEvent> _events = [];
    public IReadOnlyList<DomainEvent> Events => _events.AsReadOnly();

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    { }

    protected void AddEvent(DomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public void ClearEvents()
    {
        _events.Clear();
    }

    public override bool Equals(object? obj)
        => obj is Entity entity && Id == entity.Id;

    public bool Equals(Entity? other)
        => Equals((object?)other);

    public static bool operator ==(Entity left, Entity right) 
        => Equals(left, right);

    public static bool operator !=(Entity left, Entity right) 
        => !Equals(left, right);

    public override int GetHashCode()
        => Id.GetHashCode();
}