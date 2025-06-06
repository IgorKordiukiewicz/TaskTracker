﻿using Domain.Workflows;

namespace Application.Features.Workflows;

public record ChangeInitialWorkflowStatusCommand(Guid WorkflowId, ChangeInitialWorkflowStatusDto Model) : IRequest<Result>;

internal class ChangeInitialWorkflowStatusCommandValidator : AbstractValidator<ChangeInitialWorkflowStatusCommand>
{
    public ChangeInitialWorkflowStatusCommandValidator()
    {
        RuleFor(x => x.WorkflowId).NotEmpty();
        RuleFor(x => x.Model.StatusId).NotEmpty();
    }
}

internal class ChangeInitialWorkflowStatusHandler(IRepository<Workflow> workflowRepository) 
    : IRequestHandler<ChangeInitialWorkflowStatusCommand, Result>
{
    public async Task<Result> Handle(ChangeInitialWorkflowStatusCommand request, CancellationToken cancellationToken)
    {
        var workflow = await workflowRepository.GetById(request.WorkflowId, cancellationToken);
        if (workflow is null)
        {
            return Result.Fail(new NotFoundError<Workflow>(request.WorkflowId));
        }

        var result = workflow.ChangeInitialStatus(request.Model.StatusId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await workflowRepository.Update(workflow, cancellationToken);
        return Result.Ok();
    }
}
