namespace Domain.Events;

public record TaskCreated : DomainEvent
{
    public Guid TaskId { get; private init; }

    public TaskCreated(Guid taskId, Guid projectId, DateTime now)
        : base(projectId, now)
    {
        TaskId = taskId;
    }
}
