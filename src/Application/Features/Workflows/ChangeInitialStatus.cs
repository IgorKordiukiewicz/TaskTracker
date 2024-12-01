using Domain.Workflows;

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

internal class ChangeInitialWorkflowStatusHandler : IRequestHandler<ChangeInitialWorkflowStatusCommand, Result>
{
    private readonly IRepository<Workflow> _workflowRepository;

    public ChangeInitialWorkflowStatusHandler(IRepository<Workflow> workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<Result> Handle(ChangeInitialWorkflowStatusCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetById(request.WorkflowId, cancellationToken);
        if (workflow is null)
        {
            return Result.Fail(new NotFoundError<Workflow>(request.WorkflowId));
        }

        var result = workflow.ChangeInitialStatus(request.Model.StatusId);
        if(result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _workflowRepository.Update(workflow, cancellationToken);
        return Result.Ok();
    }
}
