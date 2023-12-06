using Application.Errors;
using Domain.Organizations;
using Domain.Projects;

namespace Application.Features.Projects;

public record GetProjectsForOrganizationQuery(Guid OrganizationId, string UserAuthId) : IRequest<Result<ProjectsVM>>;

internal class GetProjectsForOrganizationQueryValidator : AbstractValidator<GetProjectsForOrganizationQuery>
{
    public GetProjectsForOrganizationQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.UserAuthId).NotEmpty();
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
            return Result.Fail<ProjectsVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var userId = (await _dbContext.Users
            .AsNoTracking()
            .FirstAsync(x => x.AuthenticationId == request.UserAuthId)).Id;

        var projects = await _dbContext.Projects
            .Include(x => x.Members)
            .Where(x => x.OrganizationId == request.OrganizationId && x.Members.Any(xx => xx.UserId == userId))
            .Select(x => new ProjectVM
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

        return Result.Ok(new ProjectsVM(projects));
    }
}
