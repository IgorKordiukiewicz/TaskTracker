using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record UpdateTaskEstimatedTimeCommand(Guid TaskId, UpdateTaskEstimatedTimeDto Model) : IRequest<Result>;

internal class UpdateTaskEstimatedTimeCommandValidator : AbstractValidator<UpdateTaskEstimatedTimeCommand>
{
    public UpdateTaskEstimatedTimeCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Model.Minutes).NotNull().GreaterThanOrEqualTo(0);
    }
}

internal class UpdateTaskEstimatedTimeHandler(IRepository<Task> taskRepository) 
    : IRequestHandler<UpdateTaskEstimatedTimeCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskEstimatedTimeCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }
        
        task.UpdateEstimatedTime(request.Model.Minutes);
        await taskRepository.Update(task, cancellationToken);

        return Result.Ok();
    }
}