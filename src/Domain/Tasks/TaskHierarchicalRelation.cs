namespace Domain.Tasks;

public class TaskHierarchicalRelation : ValueObject
{
    public Guid ParentId { get; private init; }
    public Guid ChildId { get; private init; }

    public TaskHierarchicalRelation(Guid parentId, Guid childId)
    {
        ParentId = parentId;
        ChildId = childId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ParentId;
        yield return ChildId;
    }
}

