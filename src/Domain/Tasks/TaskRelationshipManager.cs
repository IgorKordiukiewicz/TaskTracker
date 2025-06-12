namespace Domain.Tasks;

public class TaskRelationshipManager : Entity, IAggregateRoot
{
    public Guid ProjectId { get; init; }

    private readonly List<TaskHierarchicalRelationship> _hierarchicalRelationships = [];
    public IReadOnlyList<TaskHierarchicalRelationship> HierarchicalRelationships => _hierarchicalRelationships.AsReadOnly();

    public TaskRelationshipManager(Guid projectId) 
        : base(Guid.NewGuid())
    {
        ProjectId = projectId;
    }

    public Result AddHierarchicalRelationship(Guid parentId, Guid childId, IEnumerable<Guid> projectTasksIds)
    {
        if(parentId == childId)
        {
            return Result.Fail(new DomainError("Parent task can't be the same as child task."));
        }

        if (!projectTasksIds.Contains(parentId))
        {
            return Result.Fail(new DomainError("Parent task does not belong to the project."));
        }

        if (!projectTasksIds.Contains(childId))
        {
            return Result.Fail(new DomainError("Child task does not belong to the project."));
        }

        // This check also handles cases where this relationship already exists.
        if (_hierarchicalRelationships.Any(x => x.ChildId == childId))
        {
            return Result.Fail(new DomainError("Tasks can't have multiple parents."));
        }

        if (_hierarchicalRelationships.Any(x => x.ParentId == childId && x.ChildId == parentId))
        {
            return Result.Fail(new DomainError("Reverse relationship already exists."));
        }

        _hierarchicalRelationships.Add(new TaskHierarchicalRelationship(parentId, childId));

        return Result.Ok();
    }

    public Result RemoveHierarchicalRelationship(Guid parentId, Guid childId)
    {
        var relationship = _hierarchicalRelationships.FirstOrDefault(x => x.ParentId == parentId && x.ChildId == childId);
        if(relationship is null)
        {
            return Result.Fail(new DomainError("The relationship does not exist."));
        }

        _hierarchicalRelationships.Remove(relationship);

        return Result.Ok();
    }
}
