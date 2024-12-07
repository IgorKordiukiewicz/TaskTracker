namespace Domain.Tasks;

public class TaskActivity : ValueObject
{
    public Guid TaskId { get; init; }
    public TaskProperty Property { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public DateTime OccurredAt { get; init; }

    public TaskActivity(Guid taskId, TaskProperty property, DateTime occurredAt, string? oldValue = null, string? newValue = null)
    {
        TaskId = taskId;
        Property = property;
        OldValue = oldValue;
        NewValue = newValue;
        OccurredAt = occurredAt;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TaskId;
        yield return Property;
        yield return OldValue ?? string.Empty; // TODO: Handle nullability
        yield return NewValue ?? string.Empty;
        yield return OccurredAt;
    }
}
