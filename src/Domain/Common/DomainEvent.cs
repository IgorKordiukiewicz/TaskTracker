namespace Domain.Common;

public record DomainEvent
{
    public Guid ProjectId { get; private init; }
    public DateTime OccurredAt { get; private init; }
    public EventType Type { get; private init; }

    protected DomainEvent(Guid projectId, EventType type, DateTime? now = null)
    {
        ProjectId = projectId;
        OccurredAt = now ?? DateTime.UtcNow;
        Type = type;
    }
}
