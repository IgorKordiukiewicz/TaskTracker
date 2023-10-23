namespace Domain.Common;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; private init; }

    protected Entity(Guid id)
    {
        Id = id;
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