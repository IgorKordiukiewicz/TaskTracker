﻿namespace Application.Features.Tasks;

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
            WITH RecursiveTaskHierarchy AS (
                SELECT 
                    ParentId, ChildId, IsDeleted, TaskRelationshipManagerId
                FROM 
                    TaskHierarchicalRelationships thr
                WHERE 
                    ParentId = {0}

                UNION ALL

                SELECT 
                    thr1.ParentId, thr1.ChildId, thr1.IsDeleted, thr1.TaskRelationshipManagerId
                FROM 
                    TaskHierarchicalRelationships thr1
                INNER JOIN 
                    RecursiveTaskHierarchy rth ON thr1.ParentId = rth.ChildId
            )
            SELECT * FROM RecursiveTaskHierarchy", request.TaskId)
            .IgnoreQueryFilters()
            .ToListAsync(cancellationToken);

        var childrenByParent = childrenRelationships
            .GroupBy(x => x.ParentId)
            .ToDictionary(k => k.Key, v => v.ToList());

        var childrenHierarchy = childrenRelationships.Count != 0
            ? BuildHierarchy(request.TaskId, childrenByParent) 
            : null;

        return new TaskRelationshipsVM(parentId, childrenHierarchy);
    }

    private static TaskHierarchyVM BuildHierarchy(Guid parentId, IReadOnlyDictionary<Guid, List<TaskHierarchicalRelationship>> childrenByParent)
    {
        var childrenHierarchies = new List<TaskHierarchyVM>();

        if (childrenByParent.TryGetValue(parentId, out var childrenRelationships))
        {
            foreach (var relationship in childrenRelationships)
            {
                var childrenHierarchy = BuildHierarchy(relationship.ChildId, childrenByParent);
                childrenHierarchies.Add(childrenHierarchy);
            }
        }

        return new TaskHierarchyVM(parentId, childrenHierarchies);
    }
}
