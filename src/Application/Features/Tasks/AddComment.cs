using Application.Common;
using Task = Domain.Tasks.Task;

namespace Application.Features.Tasks;

public record AddTaskCommentCommand(Guid TaskId, Guid UserId, AddTaskCommentDto Model) : IRequest<Result>;

internal class AddTaskCommentCommandValidator : AbstractValidator<AddTaskCommentCommand>
{
    public AddTaskCommentCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Model.Content).NotEmpty();
    }
}

internal class AddTaskCommentHandler(IRepository<Task> taskRepository, IDateTimeProvider dateTimeProvider) 
    : IRequestHandler<AddTaskCommentCommand, Result>
{
    public async Task<Result> Handle(AddTaskCommentCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.TaskId, cancellationToken);
        if(task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        task.AddComment(request.Model.Content, request.UserId, dateTimeProvider.Now());

        await taskRepository.Update(task, cancellationToken);
        return Result.Ok();
    }
}
