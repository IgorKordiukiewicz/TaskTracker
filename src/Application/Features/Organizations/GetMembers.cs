using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationMembersQuery(Guid OrganizationId) : IRequest<Result<OrganizationMembersVM>>;

internal class GetOrganizationMembersQueryValidator : AbstractValidator<GetOrganizationMembersQuery>
{
    public GetOrganizationMembersQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetOrganizationMembersHandler(AppDbContext dbContext) 
    : IRequestHandler<GetOrganizationMembersQuery, Result<OrganizationMembersVM>>
{
    public async Task<Result<OrganizationMembersVM>> Handle(GetOrganizationMembersQuery request, CancellationToken cancellationToken)
    {
        var organization = await dbContext.Organizations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Fail<OrganizationMembersVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var roleNameById = await dbContext.OrganizationRoles
            .Where(x => x.OrganizationId == request.OrganizationId)
            .ToDictionaryAsync(k => k.Id, v => v.Name, cancellationToken);

        var members = (await dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Id == request.OrganizationId)
            .SelectMany(x => x.Members)
            .Join(dbContext.Users,
            member => member.UserId,
            user => user.Id,
            (member, user) => new OrganizationMemberVM
            {
                Id = member.Id,
                UserId = user.Id,
                Name = user.FullName,
                Email = user.Email,
                RoleId = member.RoleId,
                RoleName = roleNameById[member.RoleId],
                Owner = user.Id == organization.OwnerId
            })
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.Name)
            .ToList();

        return Result.Ok(new OrganizationMembersVM(members));
    }
}
