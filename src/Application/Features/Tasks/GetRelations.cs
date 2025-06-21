namespace Application.Features.Tasks;

using Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Tasks.Task;

public record GetTaskRelationsQuery(Guid TaskId) : IRequest<Result<TaskRelationsVM>>;

internal class GetTaskRelationsQueryValidator : AbstractValidator<GetTaskRelationsQuery>
{
    public GetTaskRelationsQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

internal class GetTaskRelationsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetTaskRelationsQuery, Result<TaskRelationsVM>>
{
    public async Task<Result<TaskRelationsVM>> Handle(GetTaskRelationsQuery request, CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);
        if(task is null)
        {
            return Result.Fail<TaskRelationsVM>(new NotFoundError<Task>(request.TaskId));
        }

        var parentId = (await dbContext.TaskHierarchicalRelations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ChildId == request.TaskId, cancellationToken))?.ParentId ?? null;

        var childrenRelations = await dbContext.TaskHierarchicalRelations.FromSqlRaw(
            @"
            WITH RECURSIVE RecursiveTaskHierarchy AS (
                SELECT 
                    thr.""ParentId"", thr.""ChildId"", thr.""IsDeleted"", thr.""TaskRelationManagerId""
                FROM 
                    public.""TaskHierarchicalRelations"" thr
                WHERE 
                    thr.""ParentId"" = {0}

                UNION ALL

                SELECT 
                    thr1.""ParentId"", thr1.""ChildId"", thr1.""IsDeleted"", thr1.""TaskRelationManagerId""
                FROM 
                    public.""TaskHierarchicalRelations"" thr1
                INNER JOIN 
                    RecursiveTaskHierarchy rth ON thr1.""ParentId"" = rth.""ChildId""
            )
            SELECT * FROM RecursiveTaskHierarchy", request.TaskId)
            .IgnoreQueryFilters()
            .ToListAsync(cancellationToken);

        var childrenByParent = childrenRelations
            .GroupBy(x => x.ParentId)
            .ToDictionary(k => k.Key, v => v.ToList());

        var allTasksIds = childrenRelations
            .Select(x => x.ParentId)
            .Concat(childrenRelations.Select(x => x.ChildId))
            .Concat(parentId is not null ? [ parentId.Value ] : [])
            .Distinct()
            .ToHashSet();
        var taskDataById = await dbContext.Tasks
            .Where(x => x.ProjectId == task.ProjectId && allTasksIds.Contains(x.Id))
            .ToDictionaryAsync(k => k.Id, v => (v.Title, v.ShortId), cancellationToken);

        var childrenHierarchy = childrenRelations.Count != 0
            ? BuildHierarchy(request.TaskId, childrenByParent, taskDataById) 
            : null;

        var parent = parentId is not null 
            ? new TaskRelationsParentVM(parentId.Value, taskDataById[parentId.Value].Title, taskDataById[parentId.Value].ShortId) 
            : null;

        return new TaskRelationsVM(parent, childrenHierarchy);
    }

    private static TaskHierarchyVM BuildHierarchy(Guid parentId, IReadOnlyDictionary<Guid, List<TaskHierarchicalRelation>> childrenByParent, IReadOnlyDictionary<Guid, (string Title, int ShortId)> dataById)
    {
        var childrenHierarchies = new List<TaskHierarchyVM>();

        if (childrenByParent.TryGetValue(parentId, out var childrenRelations))
        {
            foreach (var relation in childrenRelations)
            {
                var childrenHierarchy = BuildHierarchy(relation.ChildId, childrenByParent, dataById);
                childrenHierarchies.Add(childrenHierarchy);
            }
        }

        var data = dataById[parentId];
        return new TaskHierarchyVM(parentId, data.Title, data.ShortId, childrenHierarchies);
    }
}
