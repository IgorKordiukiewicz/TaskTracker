using Domain.Common;

namespace Domain.Tasks;

// TODO: what would happen if you tried to remove a status that was in use by a task?
// TODO: move to separate folder?
// TODO: Add TaskTransitionStatus Value object
public class Workflow : Entity, IAggregateRoot
{
    public Guid ProjectId { get; set; }

    private readonly List<TaskStatus> _statuses = new();
    public IReadOnlyList<TaskStatus> Statuses => _statuses.AsReadOnly();

    private Workflow(Guid projectId)
        : base(Guid.NewGuid())
    {
        ProjectId = projectId;
    }

    public static Workflow Create(Guid projectId)
    {
        var toDoId = Guid.NewGuid();
        var inProgressId = Guid.NewGuid();
        var doneId = Guid.NewGuid();

        var workflow = new Workflow(projectId);
        workflow._statuses.Add(TaskStatus.Create(toDoId, new("ToDo"), new[] { inProgressId }, 0, true));
        workflow._statuses.Add(TaskStatus.Create(inProgressId, new("In Progress"), new[] { toDoId, doneId }, 1));
        workflow._statuses.Add(TaskStatus.Create(doneId, new("Done"), new[] { inProgressId }, 2));

        return workflow;
    }

    public bool DoesStatusExist(Guid statusId)
        => _statuses.Any(x => x.Id == statusId);

    public bool CanTransitionTo(Guid currentStatusId, Guid newStatusId)
    {
        var currentStatus = _statuses.Single(x => x.Id == currentStatusId);
        return currentStatus.CanTransitionTo(newStatusId);
    }
}