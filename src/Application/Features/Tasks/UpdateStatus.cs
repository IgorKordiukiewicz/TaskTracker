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

internal class UpdateTaskStatusHandler(IRepository<Domain.Tasks.Task> taskRepository, IRepository<Workflow> workflowRepository) 
    : IRequestHandler<UpdateTaskStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.TaskId, cancellationToken);
        if(task is null)
        {
            return Result.Fail(new NotFoundError<Domain.Tasks.Task>(request.TaskId));
        }

        var workflow = await workflowRepository.GetBy(x => x.ProjectId == task.ProjectId, cancellationToken);
        if(!workflow!.DoesStatusExist(request.Model.StatusId))
        {
            return Result.Fail(new NotFoundError<Domain.Workflows.TaskStatus>(request.Model.StatusId));
        }

        var result = task.UpdateStatus(request.Model.StatusId, workflow);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await taskRepository.Update(task, cancellationToken);

        return Result.Ok();
    }
}