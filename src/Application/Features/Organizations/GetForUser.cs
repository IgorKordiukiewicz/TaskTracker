namespace Application.Features.Organizations;

// TODO: Move GetForUser to Users/GetOrganizations and move Users/GetNotInOrganization to Organizations/GetAvailableUsers ?
public record GetOrganizationsForUserQuery(string UserAuthenticationId) : IRequest<Result<OrganizationsForUserVM>>;

internal class GetOrganizationsForUserQueryValidator : AbstractValidator<GetOrganizationsForUserQuery>
{
    public GetOrganizationsForUserQueryValidator()
    {
        RuleFor(x => x.UserAuthenticationId).NotEmpty();
    }
}

internal class GetOrganizationsForUserHandler : IRequestHandler<GetOrganizationsForUserQuery, Result<OrganizationsForUserVM>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationsForUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrganizationsForUserVM>> Handle(GetOrganizationsForUserQuery request, CancellationToken cancellationToken)
    {
        var userId = (await _dbContext.Users.FirstOrDefaultAsync(x => x.AuthenticationId == request.UserAuthenticationId))?.Id 
            ?? Guid.Empty;

        var organizations = await _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == userId))
            .Select(x => new OrganizationForUserVM()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

        return Result.Ok<OrganizationsForUserVM>(new(organizations));
    }
}
