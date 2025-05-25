using Domain.Events;
using Domain.Workflows;

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
    
    private readonly List<TaskComment> _comments = [];
    public IReadOnlyList<TaskComment> Comments => _comments.AsReadOnly();

    private readonly List<TaskActivity> _activities = [];
    public IReadOnlyList<TaskActivity> Activities => _activities.AsReadOnly();

    private readonly List<TaskTimeLog> _timeLogs = [];
    public IReadOnlyList<TaskTimeLog> TimeLogs => _timeLogs.AsReadOnly();

    public int? EstimatedTime { get; private set; }
    public int TotalTimeLogged => TimeLogs.Sum(x => x.Minutes);

    private Task(Guid id)
        : base(id)
    {

    }

    public static Task Create(int shortId, Guid projectId, DateTime now, string title, string description, Guid statusId, Guid? assigneeId = null, TaskPriority priority = TaskPriority.Normal)
    {
        var result = new Task(Guid.NewGuid())
        {
            ShortId = shortId,
            ProjectId = projectId,
            Title = title,
            Description = description,
            StatusId = statusId,
            AssigneeId = assigneeId,
            Priority = priority
        };

        result._activities.Add(new(result.Id, TaskProperty.Creation, now));

        result.AddEvent(new TaskCreated(result.Id, projectId, now));

        return result;
    }

    public void UpdateTitle(string title, DateTime now)
    {
        _activities.Add(new(Id, TaskProperty.Title, now, Title, title));
        Title = title;
    }

    public void UpdateDescription(string description, DateTime now)
    {
        _activities.Add(new(Id, TaskProperty.Description, now, Description, description));
        Description = description;
    }

    public Result UpdateStatus(Guid newStatusId, Workflow workflow, DateTime now)
    {
        if(!workflow.CanTransitionTo(StatusId, newStatusId))
        {
            return Result.Fail(new DomainError($"Invalid status transition"));
        }

        _activities.Add(new(Id, TaskProperty.Status, now, StatusId.ToString(), newStatusId.ToString()));
        StatusId = newStatusId;
        return Result.Ok();
    }

    public void UpdateAssignee(Guid newAssigneeId, DateTime now)
    {
        _activities.Add(new(Id, TaskProperty.Assignee, now, AssigneeId?.ToString(), newAssigneeId.ToString()));
        AssigneeId = newAssigneeId;
    }

    public void Unassign(DateTime now)
    {
        _activities.Add(new(Id, TaskProperty.Assignee, now, AssigneeId.ToString()));
        AssigneeId = null;
    }

    public void UpdatePriority(TaskPriority newPriority, DateTime now)
    {
        _activities.Add(new(Id, TaskProperty.Priority, now, Priority.ToString(), newPriority.ToString()));
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

    public void UpdateEstimatedTime(int minutes)
    {
        EstimatedTime = minutes <= 0 ? null : minutes;
    }
}