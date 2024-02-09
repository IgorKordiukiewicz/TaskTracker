namespace Domain.Workflows;

public class TaskStatusTransition : ValueObject
{
    public Guid FromStatusId { get; }
    public Guid ToStatusId { get; }

    public TaskStatusTransition(Guid fromStatusId, Guid toStatusId)
    {
        FromStatusId = fromStatusId;
        ToStatusId = toStatusId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FromStatusId;
        yield return ToStatusId;
    }
}
