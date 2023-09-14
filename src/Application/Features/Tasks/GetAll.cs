using Application.Errors;

namespace Application.Features.Tasks;

public record GetAllTasksQuery(Guid ProjectId) : IRequest<Result<TasksVM>>;

internal class GetAllTasksQueryValidator : AbstractValidator<GetAllTasksQuery>
{
    public GetAllTasksQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetAllTasksHandler : IRequestHandler<GetAllTasksQuery, Result<TasksVM>>
{
    private readonly AppDbContext _context;

    public GetAllTasksHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TasksVM>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var workflow = await _context.Workflows
            .Include(x => x.Statuses)
            .Where(x => x.ProjectId == request.ProjectId)
            .SingleOrDefaultAsync();

        if(workflow is null)
        {
            return Result.Fail<TasksVM>(new ApplicationError("Workflow with this ID does not exist."));
        }

        var statusesById = workflow.Statuses.ToDictionary(x => x.Id, x => x);

        var tasks = await _context.Tasks
            .Where(x => x.ProjectId == request.ProjectId)
            .Join(_context.TaskStatuses, 
            x => x.StatusId,
            x => x.Id, 
            (task, status) => new
            {
                Id = task.Id,
                ShortId = task.ShortId,
                Title = task.Title,
                Description = task.Description,
                Status = status.Id,
                PossibleNextStatuses = status.PossibleNextStatuses
            }).ToListAsync();

        var allTaskStatuses = workflow.Statuses
            .Select(x => new TaskStatusDetailedVM(x.Id, x.Name, x.DisplayOrder))
            .ToList();

        return new TasksVM(tasks.Select(x => new TaskVM
        {
            Id = x.Id,
            ShortId = x.ShortId,
            Title = x.Title,
            Description = x.Description,
            Status = new(x.Status, statusesById[x.Status].Name),
            PossibleNextStatuses = x.PossibleNextStatuses.Select(xx => new TaskStatusVM(xx, statusesById[xx].Name)).ToList(),
        }).ToList(), allTaskStatuses);
    }
}
