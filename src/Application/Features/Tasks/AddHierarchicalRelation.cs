using Domain.Tasks;

namespace Application.Features.Tasks;

public record AddHierarchicalTaskRelationCommand(Guid ProjectId, AddHierarchicalTaskRelationDto Model) : IRequest<Result>;

internal class AddHierarchicalTaskRelationCommandValidator : AbstractValidator<AddHierarchicalTaskRelationCommand>
{
    public AddHierarchicalTaskRelationCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Model.ParentId).NotEmpty();
        RuleFor(x => x.Model.ChildId).NotEmpty();
    }
}

internal class AddHierarchicalTaskRelationHandler(IRepository<TaskRelationManager> relationManagerRepository, AppDbContext dbContext) 
    : IRequestHandler<AddHierarchicalTaskRelationCommand, Result>
{
    public async Task<Result> Handle(AddHierarchicalTaskRelationCommand request, CancellationToken cancellationToken)
    {
        var relationManager = await relationManagerRepository.GetBy(x => x.ProjectId == request.ProjectId, cancellationToken);
        if (relationManager is null)
        {
            return Result.Fail(new NotFoundError<TaskRelationManager>($"project ID: {request.ProjectId}"));
        }

        var projectTasksIds = await dbContext.Tasks
            .Where(x => x.ProjectId == request.ProjectId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var result = relationManager.AddHierarchicalRelation(request.Model.ParentId, request.Model.ChildId, projectTasksIds);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await relationManagerRepository.Update(relationManager, cancellationToken);
        return Result.Ok();
    }
}
