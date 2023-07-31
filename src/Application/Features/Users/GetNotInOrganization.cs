namespace Application.Features.Users;

public record GetUsersNotInOrganizationQuery(Guid OrganizationId, string SearchValue) : IRequest<Result<UsersSearchVM>>;

internal class GetUsersNotInOrganizationQueryValidator : AbstractValidator<GetUsersNotInOrganizationQuery>
{
    public GetUsersNotInOrganizationQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.SearchValue).NotEmpty();
    }
}

internal class GetUsersNotInOrganizationHandler : IRequestHandler<GetUsersNotInOrganizationQuery, Result<UsersSearchVM>>
{
    private readonly AppDbContext _dbContext;

    public GetUsersNotInOrganizationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UsersSearchVM>> Handle(GetUsersNotInOrganizationQuery request, CancellationToken cancellationToken)
    {
        // TODO: Possible performance bottleneck?
        // Create a view which has a list of (UserId, UserName, OrganizationId) ?
        var unavailableUserIds = _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Id == request.OrganizationId)
            .SelectMany(x => x.Members)
            .Select(x => x.UserId);

        var searchValue = request.SearchValue.ToLower();
        var users = await _dbContext.Users.Where(x => !unavailableUserIds.Contains(x.Id) && x.Name.ToLower().Contains(searchValue))
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
