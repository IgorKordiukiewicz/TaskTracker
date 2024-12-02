using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record UpdateTaskPriorityCommand(Guid TaskId, UpdateTaskPriorityDto Model) : IRequest<Result>;

internal class UpdateTaskPriorityCommandValidator : AbstractValidator<UpdateTaskPriorityCommand>
{
    public UpdateTaskPriorityCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Model.Priority).NotNull();
    }
}

internal class UpdateTaskPriorityHandler(IRepository<Task> taskRepository) 
    : IRequestHandler<UpdateTaskPriorityCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskPriorityCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.TaskId, cancellationToken);
        if(task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        task.UpdatePriority(request.Model.Priority);
        await taskRepository.Update(task, cancellationToken);

        return Result.Ok();
    }
}
