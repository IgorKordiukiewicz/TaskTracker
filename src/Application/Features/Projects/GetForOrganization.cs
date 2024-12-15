using Domain.Organizations;

namespace Application.Features.Projects;

public record GetProjectsForOrganizationQuery(Guid OrganizationId, Guid UserId) : IRequest<Result<ProjectsVM>>;

internal class GetProjectsForOrganizationQueryValidator : AbstractValidator<GetProjectsForOrganizationQuery>
{
    public GetProjectsForOrganizationQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetProjectsForOrganizationHandler(AppDbContext dbContext) 
    : IRequestHandler<GetProjectsForOrganizationQuery, Result<ProjectsVM>>
{
    public async Task<Result<ProjectsVM>> Handle(GetProjectsForOrganizationQuery request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId, cancellationToken))
        {
            return Result.Fail<ProjectsVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var projects = await dbContext.Projects
            .AsNoTracking()
            .Include(x => x.Members)
            .Where(x => x.OrganizationId == request.OrganizationId && x.Members.Any(xx => xx.UserId == request.UserId))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var projectsIds = projects.Select(x => x.Id).ToHashSet();
        var tasksCountByProjectId = await dbContext.Tasks
            .Where(x => projectsIds.Contains(x.ProjectId))
            .GroupBy(x => x.ProjectId)
            .ToDictionaryAsync(k => k.Key, v => v.Count(), cancellationToken);

        return Result.Ok(new ProjectsVM(projects.Select(x => new ProjectVM()
        {
            Id = x.Id,
            Name = x.Name,
            MembersCount = x.Members.Count,
            TasksCount = tasksCountByProjectId.TryGetValue(x.Id, out var tasksCount) ? tasksCount : 0
        }).ToList()));
    }
}
