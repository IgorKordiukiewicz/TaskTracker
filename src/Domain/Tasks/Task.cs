using Domain.Workflows;
using Shared.Enums;

namespace Domain.Tasks;

public class Task : Entity, IAggregateRoot
{
    public int ShortId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid StatusId { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public TaskPriority Priority { get; private set; } = TaskPriority.Normal;
    
    private readonly List<TaskComment> _comments = new();
    public IReadOnlyList<TaskComment> Comments => _comments.AsReadOnly();

    private readonly List<TaskActivity> _activities = new();
    public IReadOnlyList<TaskActivity> Activities => _activities.AsReadOnly();

    private readonly List<TaskTimeLog> _timeLogs = new();
    public IReadOnlyList<TaskTimeLog> TimeLogs => _timeLogs.AsReadOnly();

    public int? EstimatedTime { get; set; }
    public int TotalTimeLogged => TimeLogs.Sum(x => x.Minutes);

    private Task(Guid id)
        : base(id)
    {

    }

    public static Task Create(int shortId, Guid projectId, string title, string description, Guid statusId, Guid? assigneeId = null, TaskPriority priority = TaskPriority.Normal)
    {
        return new(Guid.NewGuid())
        {
            ShortId = shortId,
            ProjectId = projectId,
            Title = title,
            Description = description,
            StatusId = statusId,
            AssigneeId = assigneeId,
            Priority = priority
        };
    }

    public void UpdateDescription(string description)
    {
        _activities.Add(new(Id, TaskProperty.Description, Description, description));
        Description = description;
    }

    public Result UpdateStatus(Guid newStatusId, Workflow workflow)
    {
        if(!workflow.CanTransitionTo(StatusId, newStatusId))
        {
            return Result.Fail(new DomainError($"Invalid status transition"));
        }

        _activities.Add(new(Id, TaskProperty.Status, StatusId.ToString(), newStatusId.ToString()));
        StatusId = newStatusId;
        return Result.Ok();
    }

    public void UpdateAssignee(Guid newAssigneeId)
    {
        _activities.Add(new(Id, TaskProperty.Assignee, AssigneeId?.ToString(), newAssigneeId.ToString()));
        AssigneeId = newAssigneeId;
    }

    public void Unassign()
    {
        _activities.Add(new(Id, TaskProperty.Assignee, AssigneeId.ToString()));
        AssigneeId = null;
    }

    public void UpdatePriority(TaskPriority newPriority)
    {
        _activities.Add(new(Id, TaskProperty.Priority, Priority.ToString(), newPriority.ToString()));
        Priority = newPriority;
    }

    public void AddComment(string content, Guid authorId, DateTime now)
    {
        _comments.Add(new(Id, content, authorId, now));
    }

    public void LogTime(int minutes, DateOnly day, Guid userId)
    {
        // TODO: Save as activity ?
        _timeLogs.Add(new (Id, minutes, day, userId));
    }
}