using Domain.Workflows;

namespace Application.Features.Tasks;

public record GetTasksQuery(Guid ProjectId, IEnumerable<int> ShortIds) : IRequest<Result<TasksVM>>;

internal class GetTasksQueryValidator : AbstractValidator<GetTasksQuery>
{
    public GetTasksQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetTasksHandler : IRequestHandler<GetTasksQuery, Result<TasksVM>>
{
    private readonly AppDbContext _context;

    public GetTasksHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TasksVM>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var workflow = await _context.Workflows
            .AsNoTracking()
            .Include(x => x.Statuses)
            .Include(x => x.Transitions)
            .Where(x => x.ProjectId == request.ProjectId)
            .SingleOrDefaultAsync();

        if(workflow is null)
        {
            return Result.Fail<TasksVM>(new NotFoundError<Workflow>($"Project ID: {request.ProjectId}"));
        }

        var statusesById = workflow.Statuses.ToDictionary(x => x.Id, x => x);

        var tasks = await _context.Tasks
            .Include(x => x.TimeLogs)
            .Where(x => x.ProjectId == request.ProjectId && (!request.ShortIds.Any() || request.ShortIds.Contains(x.ShortId)))
            .Join(_context.TaskStatuses, 
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
            .ToListAsync();

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
