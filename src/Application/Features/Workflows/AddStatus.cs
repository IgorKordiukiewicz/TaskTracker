using Application.Data.Repositories;
using Application.Errors;
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

internal class AddWorkflowTaskStatusHandler : IRequestHandler<AddWorkflowTaskStatusCommand, Result>
{
    private readonly IRepository<Workflow> _workflowRepository;

    public AddWorkflowTaskStatusHandler(IRepository<Workflow> workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<Result> Handle(AddWorkflowTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetById(request.WorkflowId);
        if (workflow is null)
        {
            return Result.Fail(new NotFoundError<Workflow>(request.WorkflowId));
        }

        var result = workflow.AddStatus(request.Model.Name);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        await _workflowRepository.Update(workflow);
        return Result.Ok();
    }
}
