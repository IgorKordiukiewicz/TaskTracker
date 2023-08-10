namespace Application.Features.Projects;

public record GetProjectsForOrganizationQuery(Guid OrganizationId) : IRequest<Result<ProjectsVM>>;

internal class GetProjectsForOrganizationQueryValidator : AbstractValidator<GetProjectsForOrganizationQuery>
{
    public GetProjectsForOrganizationQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetProjectsForOrganizationHandler : IRequestHandler<GetProjectsForOrganizationQuery, Result<ProjectsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetProjectsForOrganizationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ProjectsVM>> Handle(GetProjectsForOrganizationQuery request, CancellationToken cancellationToken)
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
