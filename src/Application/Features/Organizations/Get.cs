namespace Application.Features.Organizations;

public record GetOrganizationsQuery(Guid UserId) : IRequest<Result<OrganizationsVM>>;

internal class GetOrganizationsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetOrganizationsQuery, Result<OrganizationsVM>>
{
    public async Task<Result<OrganizationsVM>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var organizations = await dbContext.Organizations
            .AsNoTracking()
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == request.UserId))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var organizationsIds = organizations.Select(x => x.Id).ToHashSet();
        var projectsCountByOrganizationId = await dbContext.Projects
            .Where(x => organizationsIds.Contains(x.OrganizationId))
            .GroupBy(x => x.OrganizationId)
            .ToDictionaryAsync(k => k.Key, v => v.Count(), cancellationToken);

        return Result.Ok<OrganizationsVM>(new(organizations.Select(x => new OrganizationVM()
        {
            Id = x.Id,
            Name = x.Name,
            MembersCount = x.Members.Count,
            ProjectsCount = projectsCountByOrganizationId.TryGetValue(x.Id, out var projectsCount) ? projectsCount : 0
        }).ToList()));
    }
}
