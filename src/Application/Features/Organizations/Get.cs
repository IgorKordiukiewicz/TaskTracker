namespace Application.Features.Organizations;

public record GetOrganizationsQuery(Guid UserId) : IRequest<Result<OrganizationsVM>>;

internal class GetOrganizationsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetOrganizationsQuery, Result<OrganizationsVM>>
{
    public async Task<Result<OrganizationsVM>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var organizations = await dbContext.Organizations
            .Include(x => x.Members)
            .Where(x => x.Members.Any(xx => xx.UserId == request.UserId))
            .Select(x => new OrganizationVM()
            {
                Id = x.Id,
                Name = x.Name,
            })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Result.Ok<OrganizationsVM>(new(organizations));
    }
}
