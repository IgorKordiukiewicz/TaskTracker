using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record LogTaskTimeCommand(string UserAuthenticationId, Guid TaskId, LogTaskTimeDto Model) : IRequest<Result>;

internal class LogTaskTimeCommandValidator : AbstractValidator<LogTaskTimeCommand>
{
    public LogTaskTimeCommandValidator()
    {
        RuleFor(x => x.UserAuthenticationId).NotEmpty();
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Model.Minutes).GreaterThan(0);
        RuleFor(x => x.Model.Day).NotEmpty();
    }
}

internal class LogTaskTimeHandler : IRequestHandler<LogTaskTimeCommand, Result>
{
    private readonly AppDbContext _dbContext;
    private readonly IRepository<Task> _taskRepository;

    public LogTaskTimeHandler(AppDbContext dbContext, IRepository<Task> taskRepository)
    {
        _dbContext = dbContext;
        _taskRepository = taskRepository;
    }

    public async Task<Result> Handle(LogTaskTimeCommand request, CancellationToken cancellationToken)
    {
        var userId = await _dbContext.Users
            .Where(x => x.AuthenticationId == request.UserAuthenticationId)
            .Select(x => x.Id)
            .SingleAsync();

        var task = await _taskRepository.GetById(request.TaskId);
        if (task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }
        
        task.LogTime(request.Model.Minutes, request.Model.Day, userId);
        
        await _taskRepository.Update(task);
        return Result.Ok();
    }
}