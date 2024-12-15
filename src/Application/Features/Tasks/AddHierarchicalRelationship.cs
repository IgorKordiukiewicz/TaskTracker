using Domain.Tasks;

namespace Application.Features.Tasks;

public record AddHierarchicalTaskRelationshipCommand(Guid ProjectId, AddHierarchicalTaskRelationshipDto Model) : IRequest<Result>;

internal class AddHierarchicalTaskRelationshipCommandValidator : AbstractValidator<AddHierarchicalTaskRelationshipCommand>
{
    public AddHierarchicalTaskRelationshipCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.ParentId).NotEmpty();
        RuleFor(x => x.Model.ChildId).NotEmpty();
    }
}

internal class AddHierarchicalTaskRelationshipHandler(IRepository<TaskRelationshipManager> relationshipManagerRepository, AppDbContext dbContext) 
    : IRequestHandler<AddHierarchicalTaskRelationshipCommand, Result>
{
    public async Task<Result> Handle(AddHierarchicalTaskRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationshipManager = await relationshipManagerRepository.GetBy(x => x.ProjectId == request.ProjectId, cancellationToken);
        if (relationshipManager is null)
        {
            return Result.Fail(new NotFoundError<TaskRelationshipManager>($"project ID: {request.ProjectId}"));
        }

        var projectTasksIds = await dbContext.Tasks
            .Where(x => x.ProjectId == request.ProjectId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var result = relationshipManager.AddHierarchicalRelationship(request.Model.ParentId, request.Model.ChildId, projectTasksIds);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await relationshipManagerRepository.Update(relationshipManager, cancellationToken);
        return Result.Ok();
    }
}
