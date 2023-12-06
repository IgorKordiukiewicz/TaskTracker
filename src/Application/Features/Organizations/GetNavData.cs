using Application.Errors;
using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationNavDataQuery(Guid OrganizationId) : IRequest<Result<OrganizationNavigationVM>>;

internal class GetOrganizationNavDataQueryValidator : AbstractValidator<GetOrganizationNavDataQuery>
{
    public GetOrganizationNavDataQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetOrganizationNavDataHandler : IRequestHandler<GetOrganizationNavDataQuery, Result<OrganizationNavigationVM>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationNavDataHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrganizationNavigationVM>> Handle(GetOrganizationNavDataQuery request, CancellationToken cancellationToken)
    {
        var navData = await _dbContext.Organizations
            .Where(x => x.Id == request.OrganizationId)
            .Select(x => new NavigationItemVM(x.Id, x.Name))
            .SingleOrDefaultAsync();

        if(navData is null)
        {
            return Result.Fail<OrganizationNavigationVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        return Result.Ok(new OrganizationNavigationVM(navData));
    }
}
