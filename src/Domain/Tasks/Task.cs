using Domain.Common;
using Domain.Errors;
using FluentResults;

namespace Domain.Tasks;

public class Task : Entity, IAggregateRoot
{
    public int ShortId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid StatusId { get; private set; } = default!;

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
}