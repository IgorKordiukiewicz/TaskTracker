using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetUserOrganizationPermissionsQuery(Guid UserId, Guid OrganizationId) : IRequest<Result<UserOrganizationPermissionsVM>>;

internal class GetUserOrganizationPermissionsQueryValidator : AbstractValidator<GetUserOrganizationPermissionsQuery>
{
    public GetUserOrganizationPermissionsQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetUserOrganizationPermissionsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetUserOrganizationPermissionsQuery, Result<UserOrganizationPermissionsVM>>
{
    public async Task<Result<UserOrganizationPermissionsVM>> Handle(GetUserOrganizationPermissionsQuery request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId, cancellationToken))
        {
            return Result.Fail<UserOrganizationPermissionsVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var userRoleId = await dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Id == request.OrganizationId)
            .SelectMany(x => x.Members)
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.RoleId)
            .FirstAsync(cancellationToken);

        var data = await dbContext.OrganizationRoles
            .Where(x => x.OrganizationId == request.OrganizationId && x.Id == userRoleId)
            .Select(x => new { Permissions = x.Permissions, IsOwner = x.IsOwner() })
            .FirstAsync(cancellationToken);

        return new UserOrganizationPermissionsVM(data.Permissions, data.IsOwner);
    }
}
