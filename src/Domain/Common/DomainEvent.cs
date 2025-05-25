namespace Domain.Common;

public record DomainEvent
{
    public Guid ProjectId { get; private init; }
    public DateTime OccurredAt { get; private init; }

    protected DomainEvent(Guid projectId, DateTime now)
    {
        ProjectId = projectId;
        OccurredAt = now;
    }
}
