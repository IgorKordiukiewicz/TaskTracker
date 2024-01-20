namespace Domain.Workflows;

public class TaskStatusTransition : ValueObject
{
    public Guid FromStatusId { get; private init; }
    public Guid ToStatusId { get; private init; }

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
