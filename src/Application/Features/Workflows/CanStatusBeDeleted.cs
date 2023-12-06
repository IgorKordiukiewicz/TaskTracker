using Application.Errors;
using Domain.Projects;

namespace Application.Features.Workflows;

// TODO: Return a VM with bool and optional Reason (enum?) so that UI can display why the status can't be deleted
public record CanWorkflowStatusBeDeletedQuery(Guid WorkflowId, Guid StatusId) : IRequest<Result<bool>>;

internal class CanWorkflowStatusBeDeletedQueryValidator : AbstractValidator<CanWorkflowStatusBeDeletedQuery>
{
    public CanWorkflowStatusBeDeletedQueryValidator()
    {
        RuleFor(x => x.WorkflowId).NotEmpty();
        RuleFor(x => x.StatusId).NotEmpty();
    }
}

internal class CanWorkflowStatusBeDeletedHandler : IRequestHandler<CanWorkflowStatusBeDeletedQuery, Result<bool>>
{
    private readonly AppDbContext _dbContext;

    public CanWorkflowStatusBeDeletedHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(CanWorkflowStatusBeDeletedQuery request, CancellationToken cancellationToken)
    {
        var projectId = await _dbContext.Workflows
            .Where(x => x.Id == request.WorkflowId)
            .Select(x => x.ProjectId)
            .FirstOrDefaultAsync();
        if (projectId == default)
        {
            return Result.Fail<bool>(new NotFoundError<Project>($"workflow ID: {request.WorkflowId}"));
        }

        var status = await _dbContext.TaskStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StatusId);
        if (status is null)
        {
            return Result.Fail<bool>(new NotFoundError<Domain.Workflows.TaskStatus>(request.StatusId));
        }

        var inUse = await _dbContext.Tasks.AnyAsync(x => x.ProjectId == projectId && x.StatusId == request.StatusId);
        return !inUse && !status.Initial;
    }
}
