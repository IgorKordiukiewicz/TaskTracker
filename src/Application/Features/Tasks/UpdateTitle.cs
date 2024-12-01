using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record UpdateTaskTitleCommand(Guid TaskId, UpdateTaskTitleDto Model) : IRequest<Result>;

internal class UpdateTaskTitleCommandValidator : AbstractValidator<UpdateTaskTitleCommand>
{
    public UpdateTaskTitleCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Model.Title).NotEmpty();
    }
}

internal class UpdateTaskTitleHandler : IRequestHandler<UpdateTaskTitleCommand, Result>
{
    private readonly IRepository<Task> _repository;

    public UpdateTaskTitleHandler(IRepository<Task> repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateTaskTitleCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetById(request.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        task.UpdateTitle(request.Model.Title);
        await _repository.Update(task, cancellationToken);

        return Result.Ok();
    }
}
