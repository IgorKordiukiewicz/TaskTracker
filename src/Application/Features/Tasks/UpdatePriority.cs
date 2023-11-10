using Application.Data.Repositories;
using Application.Errors;
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

internal class UpdateTaskPriorityHandler : IRequestHandler<UpdateTaskPriorityCommand, Result>
{
    private readonly IRepository<Task> _taskRepository;

    public UpdateTaskPriorityHandler(IRepository<Task> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result> Handle(UpdateTaskPriorityCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(request.TaskId);
        if(task is null)
        {
            return Result.Fail(new ApplicationError("Task with this ID does not exist."));
        }

        task.UpdatePriority(request.Model.Priority);
        await _taskRepository.Update(task);

        return Result.Ok();
    }
}
