namespace Application.Features.Organizations;

public record GetOrganizationsForUserQuery(Guid UserId) : IRequest<Result<OrganizationsForUserVM>>;

internal class GetOrganizationsForUserQueryValidator : AbstractValidator<GetOrganizationsForUserQuery>
{
    public GetOrganizationsForUserQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty(); // TODO: is it necessary?
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
        var organizations = await _dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == request.UserId))
            .Select(x => new OrganizationForUserVM()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

        return Result.Ok<OrganizationsForUserVM>(new(organizations));
    }
}
