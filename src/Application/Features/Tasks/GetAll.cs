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
        var tasks = await _context.Tasks
            .Where(x => x.ProjectId == request.ProjectId)
            .Join(_context.TaskStates, 
            x => x.StateId,
            x => x.Id, 
            (task, state) => new TaskVM
            {
                Id = task.Id,
                ShortId = task.ShortId,
                Title = task.Title,
                Description = task.Description,
                State = new(state.Name.Value)
            }).ToListAsync();

        return new TasksVM(tasks);
    }
}
