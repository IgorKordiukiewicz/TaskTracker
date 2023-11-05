using Application.Errors;

namespace Application.Features.Projects;

public record GetProjectNavDataQuery(Guid ProjectId) : IRequest<Result<ProjectNavigationVM>>;

internal class GetProjectNavDataQueryValidator : AbstractValidator<GetProjectNavDataQuery>
{
    public GetProjectNavDataQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectNavDataHandler : IRequestHandler<GetProjectNavDataQuery, Result<ProjectNavigationVM>>
{
    private readonly AppDbContext _dbContext;

    public GetProjectNavDataHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ProjectNavigationVM>> Handle(GetProjectNavDataQuery request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId))
        {
            return Result.Fail<ProjectNavigationVM>(new ApplicationError("Project with this ID does not exist."));
        }

        var navData = await _dbContext.Projects
            .Where(x => x.Id == request.ProjectId)
            .Join(_dbContext.Organizations,
            project => project.OrganizationId,
            organization => organization.Id,
            (project, organization) => new ProjectNavigationVM(new(project.Id, project.Name), new(organization.Id, organization.Name)))
            .SingleAsync();

        return navData;
    }
}
