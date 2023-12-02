using Domain.Common;
using Domain.Errors;
using Domain.Workflows;
using FluentResults;
using Shared.Enums;

namespace Domain.Tasks;

public class Task : Entity, IAggregateRoot
{
    public int ShortId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid StatusId { get; private set; } = default!;
    public Guid? AssigneeId { get; private set; } = null;
    public TaskPriority Priority { get; private set; } = TaskPriority.Normal;


    private readonly List<TaskComment> _comments = new();
    public IReadOnlyList<TaskComment> Comments => _comments.AsReadOnly();

    private Task(Guid id)
        : base(id)
    {

    }

    // TODO: Remove unnecessary factory methods
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

    public Result UpdateStatus(Guid newStatusId, Workflow workflow)
    {
        if(!workflow.CanTransitionTo(StatusId, newStatusId))
        {
            return Result.Fail(new DomainError($"Invalid status transition"));
        }

        StatusId = newStatusId;
        return Result.Ok();
    }

    public void UpdateAssignee(Guid? newAssigneeId)
    {
        AssigneeId = newAssigneeId;
    }

    public void Unassign()
    {
        AssigneeId = null;
    }

    public void UpdatePriority(TaskPriority newPriority)
    {
        Priority = newPriority;
    }

    public void AddComment(string content, Guid authorId, DateTime now)
    {
        _comments.Add(new(Id, content, authorId, now));
    }
}