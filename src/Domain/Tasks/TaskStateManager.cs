using Domain.Common;

namespace Domain.Tasks;

// TODO: what would happen if you tried to remove a state that was in use by a task?
// TODO: move to separate folder?
public class TaskStatesManager : Entity, IAggregateRoot
{
    public Guid ProjectId { get; set; }

    private readonly List<TaskState> _allStates;
    public IReadOnlyList<TaskState> AllStates => _allStates.AsReadOnly();

    public TaskStatesManager(Guid projectId)
        : base(Guid.NewGuid())
    {
        ProjectId = projectId;

        var toDoId = Guid.NewGuid();
        var inProgressId = Guid.NewGuid();
        var doneId = Guid.NewGuid();

        _allStates = new List<TaskState>()
        {
            TaskState.Create(toDoId, new("ToDo"), new[] { inProgressId }, true),
            TaskState.Create(inProgressId, new("InProgress"), new[] { toDoId, doneId }),
            TaskState.Create(doneId, new("Done"), new[] { inProgressId })
        };
    } 
}
