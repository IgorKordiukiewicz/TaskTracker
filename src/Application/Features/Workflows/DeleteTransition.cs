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

internal class DeleteWorkflowTransitionHandler(IRepository<Workflow> workflowRepository)
    : IRequestHandler<DeleteWorkflowTransitionCommand, Result>
{
    public async Task<Result> Handle(DeleteWorkflowTransitionCommand request, CancellationToken cancellationToken)
    {
        var workflow = await workflowRepository.GetById(request.WorkflowId, cancellationToken);
        if (workflow is null)
        {
            return Result.Fail(new NotFoundError<Workflow>(request.WorkflowId));
        }

        var result = workflow.DeleteTransition(request.Model.FromStatusId, request.Model.ToStatusId);
        if(result.IsFailed)
        {
            return result;
        }

        await workflowRepository.Update(workflow, cancellationToken);
        return Result.Ok();
    }
}
