using Domain.Workflows;

namespace Application.Features.Workflows;

public record GetWorkflowForProjectQuery(Guid ProjectId) : IRequest<Result<WorkflowVM>>;

internal class GetWorkflowForProjectQueryValidator : AbstractValidator<GetWorkflowForProjectQuery>
{
    public GetWorkflowForProjectQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetWorkflowForProjectHandler : IRequestHandler<GetWorkflowForProjectQuery, Result<WorkflowVM>>
{
    private readonly AppDbContext _dbContext;

    public GetWorkflowForProjectHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<WorkflowVM>> Handle(GetWorkflowForProjectQuery request, CancellationToken cancellationToken)
    {
        var workflow = await _dbContext.Workflows
            .AsNoTracking()
            .Include(x => x.Statuses)
            .Include(x => x.Transitions)
            .SingleOrDefaultAsync(x => x.ProjectId == request.ProjectId, cancellationToken);
        if(workflow is null)
        {
            return Result.Fail<WorkflowVM>(new NotFoundError<Workflow>($"project ID: {request.ProjectId}"));
        }

        var usedStatusesIds = _dbContext.Tasks.Select(x => x.StatusId)
            .Distinct()
            .ToHashSet();

        var statuses = workflow.Statuses.Select(x =>
            new WorkflowTaskStatusVM
            {
                Id = x.Id,
                Name = x.Name,
                Initial = x.Initial,
                DeletionEligibility = x.Initial ? TaskStatusDeletionEligibility.Initial : 
                    (usedStatusesIds.Contains(x.Id) ? TaskStatusDeletionEligibility.InUse : TaskStatusDeletionEligibility.Eligible)
            }).ToList();

        var transitions = workflow.Transitions.Select(x =>
            new WorkflowTaskStatusTransitionVM(x.FromStatusId, x.ToStatusId)).ToList();

        return new WorkflowVM(workflow.Id, statuses, transitions);
    }
}
