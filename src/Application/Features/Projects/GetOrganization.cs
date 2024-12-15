using Domain.Projects;

namespace Application.Features.Projects;

public record GetProjectOrganizationQuery(Guid ProjectId) : IRequest<Result<ProjectOrganizationVM>>;

internal class GetProjectOrganizationQueryValidator : AbstractValidator<GetProjectOrganizationQuery>
{
    public GetProjectOrganizationQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectOrganizationHandler(AppDbContext dbContext) 
    : IRequestHandler<GetProjectOrganizationQuery, Result<ProjectOrganizationVM>>
{
    public async Task<Result<ProjectOrganizationVM>> Handle(GetProjectOrganizationQuery request, CancellationToken cancellationToken)
    {
        var organizationId = await dbContext.Projects
            .Where(x => x.Id == request.ProjectId)
            .Select(x => x.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);
        if(organizationId == default)
        {
            return Result.Fail<ProjectOrganizationVM>(new NotFoundError<Project>(request.ProjectId));
        }

        return new ProjectOrganizationVM(organizationId);
    }
}
