using Domain.Workflows;

namespace Application.Features.Workflows;

public record AddWorkflowTaskStatusCommand(Guid WorkflowId, AddWorkflowStatusDto Model) : IRequest<Result>;

internal class AddWorkflowTaskStatusCommandValidator : AbstractValidator<AddWorkflowTaskStatusCommand>
{
    public AddWorkflowTaskStatusCommandValidator()
    {
        RuleFor(x =>  x.WorkflowId).NotEmpty();
        RuleFor(x => x.Model.Name).NotEmpty();
    }
}

internal class AddWorkflowTaskStatusHandler(IRepository<Workflow> workflowRepository) 
    : IRequestHandler<AddWorkflowTaskStatusCommand, Result>
{
    public async Task<Result> Handle(AddWorkflowTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var workflow = await workflowRepository.GetById(request.WorkflowId, cancellationToken);
        if (workflow is null)
        {
            return Result.Fail(new NotFoundError<Workflow>(request.WorkflowId));
        }

        var result = workflow.AddStatus(request.Model.Name);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await workflowRepository.Update(workflow, cancellationToken);
        return Result.Ok();
    }
}
