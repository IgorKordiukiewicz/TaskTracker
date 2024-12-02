using Domain.Workflows;

namespace Application.Features.Workflows;

public record AddWorkflowTransitionCommand(Guid WorkflowId, AddWorkflowTransitionDto Model) : IRequest<Result>;

internal class AddWorkflowTransitionCommandValidator : AbstractValidator<AddWorkflowTransitionCommand>
{
    public AddWorkflowTransitionCommandValidator()
    {
        RuleFor(x => x.WorkflowId).NotEmpty();
        RuleFor(x => x.Model.FromStatusId).NotEmpty();
        RuleFor(x => x.Model.ToStatusId).NotEmpty();
    }
}

internal class AddWorkflowTransitionHandler(IRepository<Workflow> workflowRepository) 
    : IRequestHandler<AddWorkflowTransitionCommand, Result>
{
    public async Task<Result> Handle(AddWorkflowTransitionCommand request, CancellationToken cancellationToken)
    {
        var workflow = await workflowRepository.GetById(request.WorkflowId, cancellationToken);
        if (workflow is null)
        {
            return Result.Fail(new NotFoundError<Workflow>(request.WorkflowId));
        }

        var result = workflow.AddTransition(request.Model.FromStatusId, request.Model.ToStatusId);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await workflowRepository.Update(workflow, cancellationToken);
        return Result.Ok();
    }
}
