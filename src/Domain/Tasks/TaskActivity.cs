using Shared.Enums;

namespace Domain.Tasks;

public class TaskActivity : ValueObject
{
    public Guid TaskId { get; set; }
    public TaskProperty Property { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime OccurredAt { get; set; }

    public TaskActivity(Guid taskId, TaskProperty property, string? oldValue = null, string? newValue = null)
    {
        TaskId = taskId;
        Property = property;
        OldValue = oldValue;
        NewValue = newValue;
        OccurredAt = DateTime.Now;
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
