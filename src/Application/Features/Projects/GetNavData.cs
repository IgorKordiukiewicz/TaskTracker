using Domain.Projects;

namespace Application.Features.Projects;

public record GetProjectNavDataQuery(Guid ProjectId) : IRequest<Result<ProjectNavigationVM>>;

internal class GetProjectNavDataQueryValidator : AbstractValidator<GetProjectNavDataQuery>
{
    public GetProjectNavDataQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectNavDataHandler(AppDbContext dbContext) 
    : IRequestHandler<GetProjectNavDataQuery, Result<ProjectNavigationVM>>
{
    public async Task<Result<ProjectNavigationVM>> Handle(GetProjectNavDataQuery request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken))
        {
            return Result.Fail<ProjectNavigationVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var navData = await dbContext.Projects
            .Where(x => x.Id == request.ProjectId)
            .Join(dbContext.Organizations,
            project => project.OrganizationId,
            organization => organization.Id,
            (project, organization) => new ProjectNavigationVM(new(project.Id, project.Name), new(organization.Id, organization.Name)))
            .SingleAsync(cancellationToken);

        return navData;
    }
}
