using Domain.Common;
using Domain.Errors;
using Domain.Workflows;
using FluentResults;

namespace Domain.Tasks;

public class Task : Entity, IAggregateRoot
{
    public int ShortId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid StatusId { get; private set; } = default!;

    private readonly List<TaskComment> _comments = new();
    public IReadOnlyList<TaskComment> Comments => _comments.AsReadOnly();

    private Task(Guid id)
        : base(id)
    {

    }

    // TODO: Remove unnecessary factory methods
    public static Task Create(int shortId, Guid projectId, string title, string description, Guid statusId)
    {
        return new(Guid.NewGuid())
        {
            ShortId = shortId,
            ProjectId = projectId,
            Title = title,
            Description = description,
            StatusId = statusId
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

    public void AddComment(string content, Guid authorId)
    {
        _comments.Add(new(Id, content, authorId));
    }
}