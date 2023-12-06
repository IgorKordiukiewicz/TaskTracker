using Application.Data.Repositories;
using Application.Errors;
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

internal class UpdateTaskDescriptionHandler : IRequestHandler<UpdateTaskDescriptionCommand, Result>
{
    private readonly IRepository<Task> _taskRepository;

    public UpdateTaskDescriptionHandler(IRepository<Task> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result> Handle(UpdateTaskDescriptionCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(request.TaskId);
        if (task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        task.Description = request.Model.Description;
        await _taskRepository.Update(task);

        return Result.Ok();
    }
}
