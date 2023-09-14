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
    public Guid StateId { get; private set; } = default!;

    private Task(Guid id)
        : base(id)
    {

    }

    // TODO: Remove unnecessary factory methods
    public static Task Create(int shortId, Guid projectId, string title, string description, Guid stateId)
    {
        return new(Guid.NewGuid())
        {
            ShortId = shortId,
            ProjectId = projectId,
            Title = title,
            Description = description,
            StateId = stateId
        };
    }

    public Result UpdateState(Guid newStateId, Workflow workflow)
    {
        var state = workflow.AllStates.Single(x => x.Id == StateId);

        if(!workflow.AllStates.Any(x => x.Id == newStateId))
        {
            return Result.Fail(new DomainError("Invalid state key."));
        }

        if(!state.CanTransitionTo(newStateId))
        {
            return Result.Fail(new DomainError($"Invalid state transition"));
        }

        StateId = newStateId;
        return Result.Ok();
    }
}