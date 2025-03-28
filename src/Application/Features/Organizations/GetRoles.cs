﻿using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationRolesQuery(Guid OrganizationId) : IRequest<Result<RolesVM<OrganizationPermissions>>>;

internal class GetOrganizationRolesQueryValidator : AbstractValidator<GetOrganizationRolesQuery>
{
    public GetOrganizationRolesQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetOrganizationRolesHandler(AppDbContext dbContext) 
    : IRequestHandler<GetOrganizationRolesQuery, Result<RolesVM<OrganizationPermissions>>>
{
    public async Task<Result<RolesVM<OrganizationPermissions>>> Handle(GetOrganizationRolesQuery request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId, cancellationToken))
        {
            return Result.Fail<RolesVM<OrganizationPermissions>>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var roles = await dbContext.OrganizationRoles
            .Where(x => x.OrganizationId == request.OrganizationId)
            .Select(x => new RoleVM<OrganizationPermissions>()
            {
                Id = x.Id,
                Name = x.Name,
                Permissions = x.Permissions,
                Modifiable = x.IsModifiable(),
                Owner = x.IsOwner()
            })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Result.Ok(new RolesVM<OrganizationPermissions>(roles));
    }
}
