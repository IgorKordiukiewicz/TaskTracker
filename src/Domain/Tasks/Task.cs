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

    // TODO: Remove and use events from analytics instead?
    private readonly List<TaskActivity> _activities = [];
    public IReadOnlyList<TaskActivity> Activities => _activities.AsReadOnly();

    private readonly List<TaskTimeLog> _timeLogs = [];
    public IReadOnlyList<TaskTimeLog> TimeLogs => _timeLogs.AsReadOnly();

    private readonly List<TaskAttachment> _attachments = [];
    public IReadOnlyList<TaskAttachment> Attachments => _attachments.AsReadOnly();

    public int? EstimatedTime { get; private set; }
    public int TotalTimeLogged => TimeLogs.Sum(x => x.Minutes);

    private const int MaxAttachmentsCount = 10;

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
        result.AddEvent(new TaskCreated(result.Id, result.StatusId, result.AssigneeId, result.Priority, projectId, DateTime.UtcNow));

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
        AddEvent(new TaskStatusUpdated(Id, StatusId, newStatusId, ProjectId, DateTime.UtcNow));
        StatusId = newStatusId;
        return Result.Ok();
    }

    public void UpdateAssignee(Guid newAssigneeId, DateTime now)
    {
        _activities.Add(new(Id, TaskProperty.Assignee, now, AssigneeId?.ToString(), newAssigneeId.ToString()));
        AddEvent(new TaskAssigneeUpdated(Id, AssigneeId, newAssigneeId, ProjectId, DateTime.UtcNow));
        AssigneeId = newAssigneeId;
    }

    public void Unassign(DateTime now)
    {
        _activities.Add(new(Id, TaskProperty.Assignee, now, AssigneeId.ToString()));
        AddEvent(new TaskAssigneeUpdated(Id, AssigneeId, null, ProjectId, DateTime.UtcNow));
        AssigneeId = null;
    }

    public void UpdatePriority(TaskPriority newPriority, DateTime now)
    {
        _activities.Add(new(Id, TaskProperty.Priority, now, Priority.ToString(), newPriority.ToString()));
        AddEvent(new TaskPriorityUpdated(Id, Priority, newPriority, ProjectId, DateTime.UtcNow));
        Priority = newPriority;
    }

    public void AddComment(string content, Guid authorId, DateTime now)
    {
        _comments.Add(new(Id, content, authorId, now));
        AddEvent(new TaskCommentAdded(Id, authorId, ProjectId, DateTime.UtcNow));
    }

    public void LogTime(int minutes, DateOnly day, Guid userId)
    {
        _timeLogs.Add(new (Id, minutes, day, userId));
        AddEvent(new TaskTimeLogged(Id, minutes, day, ProjectId, DateTime.UtcNow));
    }

    public void UpdateEstimatedTime(int minutes)
    {
        var oldValue = EstimatedTime;
        EstimatedTime = minutes <= 0 ? null : minutes;
        AddEvent(new TaskEstimatedTimeUpdated(Id, oldValue, EstimatedTime, ProjectId, DateTime.UtcNow));
    }

    public Result AddAttachment(string fileName, long bytesLength, string contentType)
    {
        if (_attachments.Count >= MaxAttachmentsCount)
        {
            return Result.Fail(new DomainError($"Cannot add more than {MaxAttachmentsCount} attachments to a task."));
        }

        if(_attachments.Any(x => x.Name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)))
        {
            return Result.Fail(new DomainError($"Attachment with name '{fileName}' already exists."));
        }

        _attachments.Add(new TaskAttachment(Id, fileName, bytesLength, TaskAttachment.GetAttachmentType(contentType)));
        AddEvent(new TaskAttachmentAdded(Id, fileName, ProjectId, DateTime.UtcNow));

        return Result.Ok();
    }
}