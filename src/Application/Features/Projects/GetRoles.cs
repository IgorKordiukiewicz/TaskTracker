using Domain.Projects;

namespace Application.Features.Projects;

public record GetProjectRolesQuery(Guid ProjectId) : IRequest<Result<RolesVM>>;

internal class GetProjectRolesQueryValidator : AbstractValidator<GetProjectRolesQuery>
{
    public GetProjectRolesQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectRolesHandler(AppDbContext dbContext) 
    : IRequestHandler<GetProjectRolesQuery, Result<RolesVM>>
{
    public async Task<Result<RolesVM>> Handle(GetProjectRolesQuery request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken))
        {
            return Result.Fail<RolesVM>(new NotFoundError<Project>(request.ProjectId));
        }

        var roles = await dbContext.ProjectRoles
            .Where(x => x.ProjectId == request.ProjectId)
            .Select(x => new RoleVM()
            {
                Id = x.Id,
                Name = x.Name,
                Permissions = x.Permissions,
                Modifiable = x.IsModifiable(),
                Owner = x.IsOwner()
            })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Result.Ok(new RolesVM(roles));
    }
}
