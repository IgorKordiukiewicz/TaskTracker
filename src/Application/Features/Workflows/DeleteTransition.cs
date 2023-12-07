using Domain.Workflows;

namespace Application.Features.Workflows;

public record DeleteWorkflowTransitionCommand(Guid WorkflowId, DeleteWorkflowTransitionDto Model) : IRequest<Result>;

internal class DeleteWorkflowTransitionCommandValidator : AbstractValidator<DeleteWorkflowTransitionCommand>
{
    public DeleteWorkflowTransitionCommandValidator()
    {
        RuleFor(x => x.WorkflowId).NotEmpty();
        RuleFor(x => x.Model.FromStatusId).NotEmpty();
        RuleFor(x => x.Model.ToStatusId).NotEmpty();
    }
}

internal class DeleteWorkflowTransitionHandler : IRequestHandler<DeleteWorkflowTransitionCommand, Result>
{
    private readonly IRepository<Workflow> _workflowRepository;

    public DeleteWorkflowTransitionHandler(IRepository<Workflow> workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<Result> Handle(DeleteWorkflowTransitionCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetById(request.WorkflowId);
        if (workflow is null)
        {
            return Result.Fail(new NotFoundError<Workflow>(request.WorkflowId));
        }

        var result = workflow.DeleteTransition(request.Model.FromStatusId, request.Model.ToStatusId);
        if(result.IsFailed)
        {
            return result;
        }

        await _workflowRepository.Update(workflow);
        return Result.Ok();
    }
}
