namespace Domain.Tasks;

public class TaskHierarchicalRelationship : ValueObject
{
    public Guid ParentId { get; private init; }
    public Guid ChildId { get; private init; }

    public TaskHierarchicalRelationship(Guid parentId, Guid childId)
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

