namespace Application.Features.Organizations;

public record GetOrganizationMembersQuery(Guid OrganizationId) : IRequest<Result<OrganizationMembersVM>>;

internal class GetOrganizationMembersHandler : IRequestHandler<GetOrganizationMembersQuery, Result<OrganizationMembersVM>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationMembersHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrganizationMembersVM>> Handle(GetOrganizationMembersQuery request, CancellationToken cancellationToken)
    {
        var members = await _dbContext.Organizations.AsNoTracking()
            .Include(x => x.Members)
            .Where(x => x.Id == request.OrganizationId)
            .SelectMany(x => x.Members)
            .Join(_dbContext.Users,
            member => member.UserId,
            user => user.Id,
            (member, user) => new OrganizationMemberVM(member.Id, user.Name))
            .ToListAsync();

        return Result.Ok(new OrganizationMembersVM(members));
    }
}
