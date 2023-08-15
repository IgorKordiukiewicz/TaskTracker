namespace Application.Features.Users;

public record GetUsersAvailableForOrganizationInvitationQuery(Guid OrganizationId, string SearchValue) : IRequest<Result<UsersSearchVM>>;

internal class GetUsersAvailableForOrganizationInvitationQueryValidator : AbstractValidator<GetUsersAvailableForOrganizationInvitationQuery>
{
    public GetUsersAvailableForOrganizationInvitationQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.SearchValue).NotEmpty();
    }
}

internal class GetUsersAvailableForOrganizationInvitationHandler : IRequestHandler<GetUsersAvailableForOrganizationInvitationQuery, Result<UsersSearchVM>>
{
    private readonly AppDbContext _dbContext;

    public GetUsersAvailableForOrganizationInvitationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UsersSearchVM>> Handle(GetUsersAvailableForOrganizationInvitationQuery request, CancellationToken cancellationToken)
    {
        // TODO: Possible performance bottleneck?
        // Create a view which has a list of (UserId, UserName, OrganizationId) ?
        var organization = await _dbContext.Organizations.AsNoTracking()
            .Include(x => x.Invitations)
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == request.OrganizationId);
        if(organization is null)
        {
            return Result.Fail<UsersSearchVM>(new Error("Organization with this ID does not exist."));
        }

        var membersUsers = organization.Members.Select(x => x.UserId);
        var usersWithPendingInvitations = organization.Invitations.Select(x => x.UserId);
        var unavailableUsers = membersUsers.Concat(usersWithPendingInvitations);

        var searchValue = request.SearchValue.ToLower();
        var users = await _dbContext.Users.Where(x => !unavailableUsers.Contains(x.Id) && x.Name.ToLower().Contains(searchValue))
            .Take(5)
            .Select(x => new UserSearchVM
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToListAsync();

        return Result.Ok(new UsersSearchVM(users));
    }
}
