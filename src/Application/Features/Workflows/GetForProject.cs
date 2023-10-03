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
            .Include(x => x.Statuses)
            .SingleOrDefaultAsync(x => x.ProjectId == request.ProjectId);
        if(workflow is null)
        {
            return Result.Fail<WorkflowVM>($"Workflow does not exist for project with ID: {request.ProjectId}.");
        }

        return new WorkflowVM(workflow.Id, workflow.Statuses.Select(x => 
            new WorkflowTaskStatusVM(x.Id, x.Name, x.IsInitial, x.DisplayOrder, x.PossibleNextStatuses)).ToList());
    }
}
