namespace Domain.Notifications;

public enum NotificationContext
{
    Organization,
    Project,
    Task
}

public class Notification : Entity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }
    public bool Read { get; private set; } = false;

    public NotificationContext Context { get; private set; }
    public Guid ContextEntityId { get; private set; }
    public int? TaskShortId { get; private set; }

    private Notification(Guid id)
        : base(id)
    { }

    public static Notification Create(Guid userId, string message, DateTime now, NotificationContext context, Guid contextEntityId, int? taskShortId = null)
    {
        return new(Guid.NewGuid())
        {
            UserId = userId,
            Message = message,
            OccurredAt = now,
            Context = context,
            ContextEntityId = contextEntityId,
            TaskShortId = taskShortId
        };
    }

    public void MarkAsRead()
    {
        // TODO: fail if already read?
        Read = true;
    }
}