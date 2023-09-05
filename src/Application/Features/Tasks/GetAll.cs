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
            .Select(x => new TaskVM
            {
                Id = x.Id,
                ShortId = x.ShortId,
                Title = x.Title,
                Description = x.Description
            })
            .ToListAsync();
        return new TasksVM(tasks);
    }
}
