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
            .Include(x => x.AllStates)
            .Where(x => x.ProjectId == request.ProjectId)
            .SingleOrDefaultAsync();

        if(workflow is null)
        {
            return Result.Fail<TasksVM>(new ApplicationError("Workflow with this ID does not exist."));
        }

        var statesById = workflow.AllStates.ToDictionary(x => x.Id, x => x);

        var tasks = await _context.Tasks
            .Where(x => x.ProjectId == request.ProjectId)
            .Join(_context.TaskStates, 
            x => x.StateId,
            x => x.Id, 
            (task, state) => new
            {
                Id = task.Id,
                ShortId = task.ShortId,
                Title = task.Title,
                Description = task.Description,
                State = state.Id,
                AvailableStates = state.PossibleNextStates
            }).ToListAsync();

        var allTaskStates = workflow.AllStates
            .Select(x => new TaskStateDetailedVM(x.Id, x.Name.Value, x.DisplayOrder))
            .ToList();

        return new TasksVM(tasks.Select(x => new TaskVM
        {
            Id = x.Id,
            ShortId = x.ShortId,
            Title = x.Title,
            Description = x.Description,
            State = new(x.State, statesById[x.State].Name.Value),
            AvailableStates = x.AvailableStates.Select(xx => new TaskStateVM(xx, statesById[xx].Name.Value)).ToList(),
        }).ToList(), allTaskStates);
    }
}
