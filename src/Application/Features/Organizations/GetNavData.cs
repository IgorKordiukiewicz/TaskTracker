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

internal class GetOrganizationNavDataHandler(AppDbContext dbContext) 
    : IRequestHandler<GetOrganizationNavDataQuery, Result<OrganizationNavigationVM>>
{
    public async Task<Result<OrganizationNavigationVM>> Handle(GetOrganizationNavDataQuery request, CancellationToken cancellationToken)
    {
        var navData = await dbContext.Organizations
            .Where(x => x.Id == request.OrganizationId)
            .Select(x => new NavigationItemVM(x.Id, x.Name))
            .SingleOrDefaultAsync(cancellationToken);

        if(navData is null)
        {
            return Result.Fail<OrganizationNavigationVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        return Result.Ok(new OrganizationNavigationVM(navData));
    }
}
