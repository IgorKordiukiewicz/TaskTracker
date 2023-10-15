﻿using Application.Data.Repositories;
using Application.Errors;
using Domain.Tasks;

namespace Application.Features.Tasks;

public record UpdateTaskStatusCommand(Guid TaskId, Guid NewStatusId) : IRequest<Result>;

internal class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.NewStatusId).NotEmpty();
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
            return Result.Fail(new ApplicationError("Task with this ID does not exist."));
        }

        var workflow = await _workflowRepository.GetBy(x => x.ProjectId == task.ProjectId);
        if(!workflow!.DoesStatusExist(request.NewStatusId))
        {
            return Result.Fail(new ApplicationError("Status with this ID does not exist."));
        }

        var result = task.UpdateStatus(request.NewStatusId, workflow!);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _taskRepository.Update(task);

        return Result.Ok();
    }
}