using Domain.Common;

namespace Domain.Tasks;

public class TaskStatusTransition : ValueObject
{
    public Guid FromStatusId { get; private init; }
    public Guid ToStatusId { get; private init; }

    public TaskStatusTransition(Guid fromStatusId, Guid toStatusId)
    {
        FromStatusId = fromStatusId;
        ToStatusId = toStatusId;
    }

    protected override IEnumerable<object> GetEqualityComponents() // TODO: Unit tests?
    {
        yield return FromStatusId;
        yield return ToStatusId;
    }
}
