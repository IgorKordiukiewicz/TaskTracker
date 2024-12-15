using Application.Common;
using Domain.Errors;
using Domain.Workflows;
using Infrastructure.Extensions;

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

internal class DeleteWorkflowStatusHandler(IRepository<Workflow> workflowRepository, AppDbContext dbContext, ITasksBoardLayoutService tasksBoardLayoutService) 
    : IRequestHandler<DeleteWorkflowStatusCommand, Result>
{
    public async Task<Result> Handle(DeleteWorkflowStatusCommand request, CancellationToken cancellationToken)
    {
        var workflow = await workflowRepository.GetById(request.WorkflowId, cancellationToken);
        if(workflow is null)
        {
            return Result.Fail(new NotFoundError<Workflow>(request.WorkflowId));
        }

        if(await dbContext.Tasks.AnyAsync(x => x.ProjectId == workflow.ProjectId && x.StatusId == request.Model.StatusId, cancellationToken))
        {
            return Result.Fail(new DomainError("Status in use can't be deleted."));
        }

        var result = workflow.DeleteStatus(request.Model.StatusId);
        if(result.IsFailed)
        {
            return result;
        }

        var transactionResult = await dbContext.ExecuteTransaction(async () =>
        {
            await workflowRepository.Update(workflow, cancellationToken);
            await tasksBoardLayoutService.HandleChanges(workflow.ProjectId,
                layout => layout.DeleteStatus(request.Model.StatusId), cancellationToken);
        });
       
        if(transactionResult.IsFailed)
        {
            return Result.Fail(transactionResult.Errors);
        }

        return Result.Ok();
    }
}
