namespace Application.Features.Projects;

public record GetProjectsQuery(Guid UserId) : IRequest<Result<ProjectsVM>>;

internal class GetProjectsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetProjectsQuery, Result<ProjectsVM>>
{
    public async Task<Result<ProjectsVM>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await dbContext.Projects
            .AsNoTracking()
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == request.UserId))
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
