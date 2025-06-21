
namespace Application.Features.Tasks;

public record GetTaskAvailableChildrenQuery(Guid Id) : IRequest<Result<TaskAvailableChildrenVM>>;

internal class GetTaskAvailableChildrenQueryValidator : AbstractValidator<GetTaskAvailableChildrenQuery>
{
    public GetTaskAvailableChildrenQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

internal class GetTaskAvailableChildrenHandler(AppDbContext dbContext)
    : IRequestHandler<GetTaskAvailableChildrenQuery, Result<TaskAvailableChildrenVM>>
{
    public async Task<Result<TaskAvailableChildrenVM>> Handle(GetTaskAvailableChildrenQuery request, CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if(task is null)
        {
            return Result.Fail<TaskAvailableChildrenVM>(new NotFoundError<Domain.Tasks.Task>(request.Id));
        }

        // Exclude tasks that are the given task's parent/children or already have a parent
        var projectsTaskIds = await dbContext.Tasks
            .Where(x => x.ProjectId == task.ProjectId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var currentRelations = await dbContext.TaskHierarchicalRelations
            .Where(x => x.ParentId == task.Id || x.ChildId == task.Id)
            .ToListAsync(cancellationToken);

        var currentRelationsTaskIds = currentRelations
            .Select(x => x.ParentId)
            .Concat(currentRelations.Select(x => x.ChildId))
            .Distinct()
            .ToList();

        var tasksWithParentsIds = await dbContext.TaskHierarchicalRelations
            .Where(x => projectsTaskIds.Contains(x.ParentId))
            .Select(x => x.ChildId)
            .ToListAsync(cancellationToken);

        var excludeTaskIds = currentRelationsTaskIds
            .Concat(tasksWithParentsIds)
            .Concat([task.Id]) // exclude self
            .ToHashSet();

        var availableChildrenIds = projectsTaskIds
            .Where(x => !excludeTaskIds.Contains(x));

        var availableChildren = await dbContext.Tasks
            .Where(x => availableChildrenIds.Contains(x.Id))
            .OrderBy(x => x.ShortId)
            .Select(x => new TaskAvailableChildVM(x.Id, x.ShortId, x.Title))
            .ToListAsync(cancellationToken);

        return Result.Ok(new TaskAvailableChildrenVM(availableChildren));
    }
}
