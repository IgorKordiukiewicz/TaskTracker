using Application.Data.Repositories;
using Application.Errors;
using Domain.Tasks;

namespace Application.Features.Tasks;

public record UpdateTaskStateCommand(Guid TaskId, Guid NewStateId) : IRequest<Result>;

internal class UpdateTaskStateCommandValidator : AbstractValidator<UpdateTaskStateCommand>
{
    public UpdateTaskStateCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.NewStateId).NotEmpty();
    }
}

internal class UpdateTaskStateHandler : IRequestHandler<UpdateTaskStateCommand, Result>
{
    private readonly IRepository<Workflow> _workflowRepository;
    private readonly IRepository<Domain.Tasks.Task> _taskRepository;

    public UpdateTaskStateHandler(IRepository<Domain.Tasks.Task> taskRepository, IRepository<Workflow> workflowRepository)
    {
        _taskRepository = taskRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result> Handle(UpdateTaskStateCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(request.TaskId);
        if(task is null)
        {
            return Result.Fail(new ApplicationError("Task with this ID does not exist."));
        }

        var workflow = await _workflowRepository.GetBy(x => x.ProjectId == task.ProjectId);

        var result = task.UpdateState(request.NewStateId, workflow!);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _taskRepository.Update(task);

        return Result.Ok();
    }
}