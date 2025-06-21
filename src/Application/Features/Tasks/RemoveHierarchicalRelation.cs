using Domain.Tasks;

namespace Application.Features.Tasks;

public record RemoveHierarchicalTaskRelationCommand(Guid ProjectId, RemoveHierarchicalTaskRelationDto Model) : IRequest<Result>;

internal class RemoveHierarchicalTaskRelationCommandValidator : AbstractValidator<RemoveHierarchicalTaskRelationCommand>
{
    public RemoveHierarchicalTaskRelationCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.ParentId).NotEmpty();
        RuleFor(x => x.Model.ChildId).NotEmpty();
    }
}

internal class RemoveHierarchicalTaskRelationHandler(IRepository<TaskRelationManager> relationManagerRepository)
    : IRequestHandler<RemoveHierarchicalTaskRelationCommand, Result>
{
    public async Task<Result> Handle(RemoveHierarchicalTaskRelationCommand request, CancellationToken cancellationToken)
    {
        var relationManager = await relationManagerRepository.GetBy(x => x.ProjectId == request.ProjectId, cancellationToken);
        if (relationManager is null)
        {
            return Result.Fail(new NotFoundError<TaskRelationManager>($"project ID: {request.ProjectId}"));
        }

        var result = relationManager.RemoveHierarchicalRelation(request.Model.ParentId, request.Model.ChildId);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await relationManagerRepository.Update(relationManager, cancellationToken);
        return Result.Ok();
    }
}
