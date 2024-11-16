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

internal class GetUserOrganizationPermissionsHandler : IRequestHandler<GetUserOrganizationPermissionsQuery, Result<UserOrganizationPermissionsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetUserOrganizationPermissionsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserOrganizationPermissionsVM>> Handle(GetUserOrganizationPermissionsQuery request, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId))
        {
            return Result.Fail<UserOrganizationPermissionsVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var userRoleId = await _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Id == request.OrganizationId)
            .SelectMany(x => x.Members)
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.RoleId)
            .FirstAsync();

        var permissions = await _dbContext.OrganizationRoles
            .Where(x => x.OrganizationId == request.OrganizationId && x.Id == userRoleId)
            .Select(x => x.Permissions)
            .FirstAsync();

        return new UserOrganizationPermissionsVM(permissions);
    }
}
