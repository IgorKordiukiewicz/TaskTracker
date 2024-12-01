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

internal class AddTaskCommentHandler : IRequestHandler<AddTaskCommentCommand, Result>
{
    private readonly AppDbContext _dbContext;
    private readonly IRepository<Task> _taskRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddTaskCommentHandler(AppDbContext dbContext, IRepository<Task> taskRepository, IDateTimeProvider dateTimeProvider)
    {
        _taskRepository = taskRepository;
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(AddTaskCommentCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(request.TaskId, cancellationToken);
        if(task is null)
        {
            return Result.Fail(new NotFoundError<Task>(request.TaskId));
        }

        task.AddComment(request.Model.Content, request.UserId, _dateTimeProvider.Now());

        await _taskRepository.Update(task, cancellationToken);
        return Result.Ok();
    }
}
