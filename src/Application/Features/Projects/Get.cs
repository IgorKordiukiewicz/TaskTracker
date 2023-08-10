namespace Application.Features.Projects;

public record GetProjectsQuery(Guid OrganizationId) : IRequest<Result<ProjectsVM>>; // Rename file to GetForOrg

internal class GetProjectsQueryValidator : AbstractValidator<GetProjectsQuery>
{
    public GetProjectsQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetProjectsHandler : IRequestHandler<GetProjectsQuery, Result<ProjectsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetProjectsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ProjectsVM>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId))
        {
            return Result.Fail<ProjectsVM>(new Error("Organization with this ID does not exist."));
        }

        var projects = await _dbContext.Projects
            .Where(x => x.OrganizationId == request.OrganizationId)
            .Select(x => new ProjectVM
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

        return Result.Ok(new ProjectsVM(projects));
    }
}
