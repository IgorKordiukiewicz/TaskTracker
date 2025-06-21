namespace Domain.Tasks;

public class TaskRelationManager : Entity, IAggregateRoot
{
    public Guid ProjectId { get; init; }

    private readonly List<TaskHierarchicalRelation> _hierarchicalRelations = [];
    public IReadOnlyList<TaskHierarchicalRelation> HierarchicalRelations => _hierarchicalRelations.AsReadOnly();

    public TaskRelationManager(Guid projectId) 
        : base(Guid.NewGuid())
    {
        ProjectId = projectId;
    }

    public Result AddHierarchicalRelation(Guid parentId, Guid childId, IEnumerable<Guid> projectTasksIds)
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

        // This check also handles cases where this relation already exists.
        if (_hierarchicalRelations.Any(x => x.ChildId == childId))
        {
            return Result.Fail(new DomainError("Tasks can't have multiple parents."));
        }

        if (_hierarchicalRelations.Any(x => x.ParentId == childId && x.ChildId == parentId))
        {
            return Result.Fail(new DomainError("Reverse relation already exists."));
        }

        _hierarchicalRelations.Add(new TaskHierarchicalRelation(parentId, childId));

        return Result.Ok();
    }

    public Result RemoveHierarchicalRelation(Guid parentId, Guid childId)
    {
        var relation = _hierarchicalRelations.FirstOrDefault(x => x.ParentId == parentId && x.ChildId == childId);
        if(relation is null)
        {
            return Result.Fail(new DomainError("The relation does not exist."));
        }

        _hierarchicalRelations.Remove(relation);

        return Result.Ok();
    }
}
