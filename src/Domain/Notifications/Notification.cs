namespace Domain.Notifications;

public record NotificationData(Guid UserId, string Message, DateTime OccurredAt, Guid ContextEntityId, int? TaskShortId = null);

public class Notification : Entity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }
    public bool Read { get; private set; } = false;

    public Guid ContextEntityId { get; private set; }
    public int? TaskShortId { get; private set; }

    private Notification(Guid id)
        : base(id)
    { }

    public static Notification FromData(NotificationData data)
    {
        return new(Guid.NewGuid())
        {
            UserId = data.UserId,
            Message = data.Message,
            OccurredAt = data.OccurredAt,
            ContextEntityId = data.ContextEntityId,
            TaskShortId = data.TaskShortId
        };
    }

    public void MarkAsRead()
    {
        // TODO: fail if already read?
        Read = true;
    }
}