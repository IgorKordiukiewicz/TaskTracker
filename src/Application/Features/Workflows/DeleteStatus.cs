using Domain.Errors;
using Domain.Workflows;

namespace Application.Features.Workflows;

public record DeleteWorkflowStatusCommand(Guid WorkflowId, DeleteWorkflowStatusDto Model) : IRequest<Result>;

internal class DeleteWorkflowStatusCommandValidator : AbstractValidator<DeleteWorkflowStatusCommand>
{
    public DeleteWorkflowStatusCommandValidator()
    {
        RuleFor(x => x.WorkflowId).NotEmpty();
        RuleFor(x => x.Model.StatusId).NotEmpty();
    }
}

internal class DeleteWorkflowStatusHandler : IRequestHandler<DeleteWorkflowStatusCommand, Result>
{
    private readonly IRepository<Workflow> _workflowRepository;
    private readonly AppDbContext _dbContext;

    public DeleteWorkflowStatusHandler(IRepository<Workflow> workflowRepository, AppDbContext dbContext)
    {
        _workflowRepository = workflowRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteWorkflowStatusCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetById(request.WorkflowId);
        if(workflow is null)
        {
            return Result.Fail(new NotFoundError<Workflow>(request.WorkflowId));
        }

        if(await _dbContext.Tasks.AnyAsync(x => x.ProjectId == workflow.ProjectId && x.StatusId == request.Model.StatusId))
        {
            return Result.Fail(new DomainError("Status in use can't be deleted."));
        }

        var result = workflow.DeleteStatus(request.Model.StatusId);
        if(result.IsFailed)
        {
            return result;
        }

        await _workflowRepository.Update(workflow);
        return Result.Ok();
    }
}
