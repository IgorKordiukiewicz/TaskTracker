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

internal class UpdateTaskTitleHandler(IRepository<Task> repository) 
    : IRequestHandler<UpdateTaskTitleCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskTitleCommand request, CancellationToken cancellationToken)
    {
        var task = await repository.GetById(request.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        task.UpdateTitle(request.Model.Title);
        await repository.Update(task, cancellationToken);

        return Result.Ok();
    }
}
