using Application.Errors;

namespace Application.Features.Organizations;

public record GetOrganizationMembersQuery(Guid OrganizationId) : IRequest<Result<OrganizationMembersVM>>;

internal class GetOrganizationMembersQueryValidator : AbstractValidator<GetOrganizationMembersQuery>
{
    public GetOrganizationMembersQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetOrganizationMembersHandler : IRequestHandler<GetOrganizationMembersQuery, Result<OrganizationMembersVM>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationMembersHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrganizationMembersVM>> Handle(GetOrganizationMembersQuery request, CancellationToken cancellationToken)
    {
        var organization = await _dbContext.Organizations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.OrganizationId);
        if (organization is null)
        {
            return Result.Fail<OrganizationMembersVM>(new ApplicationError("Organization with this ID does not exist."));
        }

        var members = await _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Id == request.OrganizationId)
            .SelectMany(x => x.Members)
            .Join(_dbContext.Users,
            member => member.UserId,
            user => user.Id,
            (member, user) => new OrganizationMemberVM(member.Id, user.FullName, user.Id == organization.OwnerId))
            .ToListAsync();

        return Result.Ok(new OrganizationMembersVM(members));
    }
}
