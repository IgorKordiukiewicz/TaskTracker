namespace Application.Features.Tasks;

using Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Tasks.Task;

public record GetTaskRelationshipsQuery(Guid TaskId) : IRequest<Result<TaskRelationshipsVM>>;

internal class GetTaskRelationshipsQueryValidator : AbstractValidator<GetTaskRelationshipsQuery>
{
    public GetTaskRelationshipsQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

internal class GetTaskRelationshipsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetTaskRelationshipsQuery, Result<TaskRelationshipsVM>>
{
    public async Task<Result<TaskRelationshipsVM>> Handle(GetTaskRelationshipsQuery request, CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);
        if(task is null)
        {
            return Result.Fail<TaskRelationshipsVM>(new NotFoundError<Task>(request.TaskId));
        }

        var parentId = (await dbContext.TaskHierarchicalRelationships
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ChildId == request.TaskId, cancellationToken))?.ParentId ?? null;

        var childrenRelationships = await dbContext.TaskHierarchicalRelationships.FromSqlRaw(
            @"
            WITH RECURSIVE RecursiveTaskHierarchy AS (
                SELECT 
                    thr.""ParentId"", thr.""ChildId"", thr.""IsDeleted"", thr.""TaskRelationshipManagerId""
                FROM 
                    public.""TaskHierarchicalRelationships"" thr
                WHERE 
                    thr.""ParentId"" = {0}

                UNION ALL

                SELECT 
                    thr1.""ParentId"", thr1.""ChildId"", thr1.""IsDeleted"", thr1.""TaskRelationshipManagerId""
                FROM 
                    public.""TaskHierarchicalRelationships"" thr1
                INNER JOIN 
                    RecursiveTaskHierarchy rth ON thr1.""ParentId"" = rth.""ChildId""
            )
            SELECT * FROM RecursiveTaskHierarchy", request.TaskId)
            .IgnoreQueryFilters()
            .ToListAsync(cancellationToken);

        var childrenByParent = childrenRelationships
            .GroupBy(x => x.ParentId)
            .ToDictionary(k => k.Key, v => v.ToList());

        var allTasksIds = childrenRelationships
            .Select(x => x.ParentId)
            .Concat(childrenRelationships.Select(x => x.ChildId))
            .Concat(parentId is not null ? [ parentId.Value ] : [])
            .Distinct()
            .ToHashSet();
        var taskDataById = await dbContext.Tasks
            .Where(x => x.ProjectId == task.ProjectId && allTasksIds.Contains(x.Id))
            .ToDictionaryAsync(k => k.Id, v => (v.Title, v.ShortId), cancellationToken);

        var childrenHierarchy = childrenRelationships.Count != 0
            ? BuildHierarchy(request.TaskId, childrenByParent, taskDataById) 
            : null;

        var parent = parentId is not null 
            ? new TaskRelationshipsParentVM(parentId.Value, taskDataById[parentId.Value].Title, taskDataById[parentId.Value].ShortId) 
            : null;

        return new TaskRelationshipsVM(parent, childrenHierarchy);
    }

    private static TaskHierarchyVM BuildHierarchy(Guid parentId, IReadOnlyDictionary<Guid, List<TaskHierarchicalRelationship>> childrenByParent, IReadOnlyDictionary<Guid, (string Title, int ShortId)> dataById)
    {
        var childrenHierarchies = new List<TaskHierarchyVM>();

        if (childrenByParent.TryGetValue(parentId, out var childrenRelationships))
        {
            foreach (var relationship in childrenRelationships)
            {
                var childrenHierarchy = BuildHierarchy(relationship.ChildId, childrenByParent, dataById);
                childrenHierarchies.Add(childrenHierarchy);
            }
        }

        var data = dataById[parentId];
        return new TaskHierarchyVM(parentId, data.Title, data.ShortId, childrenHierarchies);
    }
}
