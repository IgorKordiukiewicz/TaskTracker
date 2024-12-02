using Domain.Workflows;
using OneOf;

namespace Application.Features.Tasks;

public record GetTasksQuery(Guid ProjectId, OneOf<int, IEnumerable<Guid>> Ids) : IRequest<Result<TasksVM>>;

internal class GetTasksQueryValidator : AbstractValidator<GetTasksQuery>
{
    public GetTasksQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Ids).NotNull();
    }
}

internal class GetTasksHandler(AppDbContext context) 
    : IRequestHandler<GetTasksQuery, Result<TasksVM>>
{
    public async Task<Result<TasksVM>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var workflow = await context.Workflows
            .AsNoTracking()
            .Include(x => x.Statuses)
            .Include(x => x.Transitions)
            .Where(x => x.ProjectId == request.ProjectId)
            .SingleOrDefaultAsync(cancellationToken);

        if(workflow is null)
        {
            return Result.Fail<TasksVM>(new NotFoundError<Workflow>($"Project ID: {request.ProjectId}"));
        }

        var statusesById = workflow.Statuses.ToDictionary(x => x.Id, x => x);

        IQueryable<Domain.Tasks.Task> query = context.Tasks
            .Include(x => x.TimeLogs)
            .Where(x => x.ProjectId == request.ProjectId);

        query = request.Ids.Match(
            shortId => query.Where(x => x.ShortId == shortId),
            ids => query.Where(x => !ids.Any() || ids.Contains(x.Id)));

        var tasks = await query
            .Join(context.TaskStatuses, 
            x => x.StatusId,
            x => x.Id, 
            (task, status) => new
            {
                task.Id,
                task.ShortId,
                task.Title,
                task.Description,
                task.AssigneeId,
                task.Priority,
                task.TotalTimeLogged,
                task.EstimatedTime,
                Status = status.Id,
            })
            .OrderByDescending(x => x.ShortId)
            .ToListAsync(cancellationToken);

        var allTaskStatuses = workflow.Statuses
            .Select(x => new TaskStatusDetailedVM(x.Id, x.Name, x.DisplayOrder))
            .ToList();

        var possibleNextStatusesByStatus = workflow.Statuses.ToDictionary(k => k.Id, _ => new List<Guid>());
        foreach(var (statusId, possibleNextStatuses) in possibleNextStatusesByStatus)
        {
            possibleNextStatuses.AddRange(workflow.Transitions.Where(x => x.FromStatusId == statusId).Select(x => x.ToStatusId));
        }

        return new TasksVM(tasks.Select(x => new TaskVM
        {
            Id = x.Id,
            ShortId = x.ShortId,
            Title = x.Title,
            Description = x.Description,
            AssigneeId = x.AssigneeId,
            Priority = x.Priority,
            Status = new(x.Status, statusesById[x.Status].Name),
            PossibleNextStatuses = possibleNextStatusesByStatus[x.Status].Select(xx => new TaskStatusVM(xx, statusesById[xx].Name)).ToList(),
            TotalTimeLogged = x.TotalTimeLogged,
            EstimatedTime = x.EstimatedTime
        }).ToList(), allTaskStatuses);
    }
}
