﻿using Domain.Projects;

namespace Application.Features.Projects;

public record GetProjectRolesQuery(Guid ProjectId) : IRequest<Result<RolesVM<ProjectPermissions>>>;

internal class GetProjectRolesQueryValidator : AbstractValidator<GetProjectRolesQuery>
{
    public GetProjectRolesQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

internal class GetProjectRolesHandler : IRequestHandler<GetProjectRolesQuery, Result<RolesVM<ProjectPermissions>>>
{
    private readonly AppDbContext _dbContext;

    public GetProjectRolesHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<RolesVM<ProjectPermissions>>> Handle(GetProjectRolesQuery request, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Projects.AnyAsync(x => x.Id == request.ProjectId, cancellationToken))
        {
            return Result.Fail<RolesVM<ProjectPermissions>>(new NotFoundError<Project>(request.ProjectId));
        }

        var roles = await _dbContext.ProjectRoles
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
