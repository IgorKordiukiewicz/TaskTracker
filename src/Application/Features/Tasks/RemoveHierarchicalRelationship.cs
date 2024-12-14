using Domain.Tasks;

namespace Application.Features.Tasks;

public record RemoveHierarchicalTaskRelationshipCommand(Guid ProjectId, RemoveHierarchicalTaskRelationshipDto Model) : IRequest<Result>;

internal class RemoveHierarchicalTaskRelationshipCommandValidator : AbstractValidator<RemoveHierarchicalTaskRelationshipCommand>
{
    public RemoveHierarchicalTaskRelationshipCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.ParentId).NotEmpty();
        RuleFor(x => x.Model.ChildId).NotEmpty();
    }
}

internal class RemoveHierarchicalTaskRelationshipHandler(IRepository<TaskRelationshipManager> relationshipManagerRepository)
    : IRequestHandler<RemoveHierarchicalTaskRelationshipCommand, Result>
{
    public async Task<Result> Handle(RemoveHierarchicalTaskRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationshipManager = await relationshipManagerRepository.GetBy(x => x.ProjectId == request.ProjectId, cancellationToken);
        if (relationshipManager is null)
        {
            return Result.Fail(new NotFoundError<TaskRelationshipManager>($"project ID: {request.ProjectId}"));
        }

        var result = relationshipManager.RemoveHierarchicalRelationship(request.Model.ParentId, request.Model.ChildId);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await relationshipManagerRepository.Update(relationshipManager, cancellationToken);
        return Result.Ok();
    }
}
