using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record LogTaskTimeCommand(Guid UserId, Guid TaskId, LogTaskTimeDto Model) : IRequest<Result>;

internal class LogTaskTimeCommandValidator : AbstractValidator<LogTaskTimeCommand>
{
    public LogTaskTimeCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Model.Minutes).GreaterThan(0);
        RuleFor(x => x.Model.Day).NotEmpty();
    }
}

internal class LogTaskTimeHandler(IRepository<Task> taskRepository) 
    : IRequestHandler<LogTaskTimeCommand, Result>
{
    public async Task<Result> Handle(LogTaskTimeCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }
        
        task.LogTime(request.Model.Minutes, DateOnly.FromDateTime(request.Model.Day), request.UserId);
        
        await taskRepository.Update(task, cancellationToken);
        return Result.Ok();
    }
}