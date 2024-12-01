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

internal class AddHierarchicalTaskRelationshipHandler : IRequestHandler<AddHierarchicalTaskRelationshipCommand, Result>
{
    private readonly IRepository<TaskRelationshipManager> _relationshipManagerRepository;
    private readonly AppDbContext _dbContext;

    public AddHierarchicalTaskRelationshipHandler(IRepository<TaskRelationshipManager> relationshipManagerRepository, AppDbContext dbContext)
    {
        _relationshipManagerRepository = relationshipManagerRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddHierarchicalTaskRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationshipManager = await _relationshipManagerRepository.GetBy(x => x.ProjectId == request.ProjectId, cancellationToken);
        if (relationshipManager is null)
        {
            return Result.Fail(new NotFoundError<TaskRelationshipManager>($"project ID: {request.ProjectId}"));
        }

        var projectTasksIds = await _dbContext.Tasks
            .Where(x => x.ProjectId == request.ProjectId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var result = relationshipManager.AddHierarchicalRelationship(request.Model.ParentId, request.Model.ChildId, projectTasksIds);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _relationshipManagerRepository.Update(relationshipManager, cancellationToken);
        return Result.Ok();
    }
}
