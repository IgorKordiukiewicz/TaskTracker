using Domain.Common;

namespace Domain.Tasks;

// TODO: what would happen if you tried to remove a state that was in use by a task?
// TODO: move to separate folder?
public class TaskStatesManager : Entity, IAggregateRoot
{
    public Guid ProjectId { get; set; }

    private readonly List<TaskState> _allStates = new();
    public IReadOnlyList<TaskState> AllStates => _allStates.AsReadOnly();

    private TaskStatesManager(Guid projectId)
        : base(Guid.NewGuid())
    {
        ProjectId = projectId;
    }

    public static TaskStatesManager Create(Guid projectId)
    {
        var toDoId = Guid.NewGuid();
        var inProgressId = Guid.NewGuid();
        var doneId = Guid.NewGuid();

        var taskStatesManager = new TaskStatesManager(projectId);
        taskStatesManager._allStates.Add(TaskState.Create(toDoId, new("ToDo"), new[] { inProgressId }, 0, true));
        taskStatesManager._allStates.Add(TaskState.Create(inProgressId, new("InProgress"), new[] { toDoId, doneId }, 1));
        taskStatesManager._allStates.Add(TaskState.Create(doneId, new("Done"), new[] { inProgressId }, 2));

        return taskStatesManager;
    }
}
