using Domain.Workflows;

namespace Application.Features.Tasks;

public record UpdateTaskStatusCommand(Guid TaskId, UpdateTaskStatusDto Model) : IRequest<Result>;

internal class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Model.StatusId).NotEmpty();
    }
}

internal class UpdateTaskStatusHandler : IRequestHandler<UpdateTaskStatusCommand, Result>
{
    private readonly IRepository<Workflow> _workflowRepository;
    private readonly IRepository<Domain.Tasks.Task> _taskRepository;

    public UpdateTaskStatusHandler(IRepository<Domain.Tasks.Task> taskRepository, IRepository<Workflow> workflowRepository)
    {
        _taskRepository = taskRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(request.TaskId);
        if(task is null)
        {
            return Result.Fail(new NotFoundError<Domain.Tasks.Task>(request.TaskId));
        }

        var workflow = await _workflowRepository.GetBy(x => x.ProjectId == task.ProjectId);
        if(!workflow!.DoesStatusExist(request.Model.StatusId))
        {
            return Result.Fail(new NotFoundError<Domain.Workflows.TaskStatus>(request.Model.StatusId));
        }

        var result = task.UpdateStatus(request.Model.StatusId, workflow!);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _taskRepository.Update(task);

        return Result.Ok();
    }
}