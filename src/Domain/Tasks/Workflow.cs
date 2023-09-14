using Domain.Common;

namespace Domain.Tasks;

// TODO: what would happen if you tried to remove a status that was in use by a task?
// TODO: move to separate folder?
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
        workflow._statuses.Add(TaskStatus.Create(inProgressId, new("InProgress"), new[] { toDoId, doneId }, 1));
        workflow._statuses.Add(TaskStatus.Create(doneId, new("Done"), new[] { inProgressId }, 2));

        return workflow;
    }
}
