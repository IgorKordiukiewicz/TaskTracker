using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationRolesQuery(Guid OrganizationId) : IRequest<Result<RolesVM<OrganizationPermissions>>>;

internal class GetOrganizationRolesQueryValidator : AbstractValidator<GetOrganizationRolesQuery>
{
    public GetOrganizationRolesQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetOrganizationRolesHandler : IRequestHandler<GetOrganizationRolesQuery, Result<RolesVM<OrganizationPermissions>>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationRolesHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<RolesVM<OrganizationPermissions>>> Handle(GetOrganizationRolesQuery request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId))
        {
            return Result.Fail<RolesVM<OrganizationPermissions>>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var roles = await _dbContext.OrganizationRoles
            .Where(x => x.OrganizationId == request.OrganizationId)
            .Select(x => new RoleVM<OrganizationPermissions>()
            {
                Id = x.Id,
                Name = x.Name,
                Permissions = x.Permissions,
                Modifiable = x.IsModifiable(),
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Result.Ok(new RolesVM<OrganizationPermissions>(roles));
    }
}
