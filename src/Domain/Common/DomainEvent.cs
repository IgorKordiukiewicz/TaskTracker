namespace Domain.Common;

public record DomainEvent
{
    public Guid ProjectId { get; init; }
    public DateTime OccurredAt { get; init; }
    public EventType Type { get; init; }

    protected DomainEvent(Guid projectId, EventType type, DateTime occurredAt)
    {
        ProjectId = projectId;
        OccurredAt = occurredAt;
        Type = type;
    }
}
