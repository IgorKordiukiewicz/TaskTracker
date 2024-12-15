using Domain.Projects;

namespace Application.Features.Projects;

public record GetProjectRolesQuery(Guid ProjectId) : IRequest<Result<RolesVM<ProjectPermissions>>>;

internal class GetProjectRolesQueryValidator : AbstractValidator<GetProjectRolesQuery>
{
    public GetProjectRolesQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectRolesHandler(AppDbContext dbContext) 
    : IRequestHandler<GetProjectRolesQuery, Result<RolesVM<ProjectPermissions>>>
{
    public async Task<Result<RolesVM<ProjectPermissions>>> Handle(GetProjectRolesQuery request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken))
        {
            return Result.Fail<RolesVM<ProjectPermissions>>(new NotFoundError<Project>(request.ProjectId));
        }

        var roles = await dbContext.ProjectRoles
            .Where(x => x.ProjectId == request.ProjectId)
            .Select(x => new RoleVM<ProjectPermissions>()
            {
                Id = x.Id,
                Name = x.Name,
                Permissions = x.Permissions,
                Modifiable = x.IsModifiable(),
                Owner = x.IsOwner()
            })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Result.Ok(new RolesVM<ProjectPermissions>(roles));
    }
}
