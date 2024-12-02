using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record UpdateTaskDescriptionCommand(Guid TaskId, UpdateTaskDescriptionDto Model) : IRequest<Result>;

internal class UpdateTaskDescriptionCommandValidator : AbstractValidator<UpdateTaskDescriptionCommand>
{
    public UpdateTaskDescriptionCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Model.Description).NotEmpty();
    }
}

internal class UpdateTaskDescriptionHandler(IRepository<Task> taskRepository) 
    : IRequestHandler<UpdateTaskDescriptionCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskDescriptionCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        task.UpdateDescription(request.Model.Description);
        await taskRepository.Update(task, cancellationToken);

        return Result.Ok();
    }
}
