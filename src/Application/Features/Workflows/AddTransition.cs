using Application.Data.Repositories;
using Application.Errors;
using Domain.Tasks;

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

internal class AddWorkflowTransitionHandler : IRequestHandler<AddWorkflowTransitionCommand, Result>
{
    private readonly IRepository<Workflow> _workflowRepository;

    public AddWorkflowTransitionHandler(IRepository<Workflow> workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<Result> Handle(AddWorkflowTransitionCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetById(request.WorkflowId);
        if (workflow is null)
        {
            return Result.Fail(new ApplicationError("Workflow with this ID does not exist."));
        }

        var result = workflow.AddTransition(request.Model.FromStatusId, request.Model.ToStatusId);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _workflowRepository.Update(workflow);
        return Result.Ok();
    }
}
