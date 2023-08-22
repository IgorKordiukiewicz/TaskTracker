using Application.Errors;

namespace Application.Features.Projects;

public record GetProjectOrganizationQuery(Guid ProjectId) : IRequest<Result<ProjectOrganizationVM>>;

internal class GetProjectOrganizationQueryValidator : AbstractValidator<GetProjectOrganizationQuery>
{
    public GetProjectOrganizationQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectOrganizationHandler : IRequestHandler<GetProjectOrganizationQuery, Result<ProjectOrganizationVM>>
{
    private readonly AppDbContext _dbContext;

    public GetProjectOrganizationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ProjectOrganizationVM>> Handle(GetProjectOrganizationQuery request, CancellationToken cancellationToken)
    {
        var organizationId = await _dbContext.Projects
            .Where(x => x.Id == request.ProjectId)
            .Select(x => x.OrganizationId)
            .FirstOrDefaultAsync();
        if(organizationId == default)
        {
            return Result.Fail<ProjectOrganizationVM>(new ApplicationError("Project with this ID does not exist."));
        }

        return new ProjectOrganizationVM(organizationId);
    }
}
