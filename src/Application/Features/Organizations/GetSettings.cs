using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationSettingsQuery(Guid OrganizationId) : IRequest<Result<OrganizationSettingsVM>>;

internal class GetOrganizationSettingsQueryValidator : AbstractValidator<GetOrganizationSettingsQuery>
{
    public GetOrganizationSettingsQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetOrganizationSettingsHandler : IRequestHandler<GetOrganizationSettingsQuery, Result<OrganizationSettingsVM>>
{
    private readonly AppDbContext _dbContext;

    public GetOrganizationSettingsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrganizationSettingsVM>> Handle(GetOrganizationSettingsQuery request, CancellationToken cancellationToken)
    {
        if(!await _dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId))
        {
            return Result.Fail<OrganizationSettingsVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        return await _dbContext.Organizations
            .Where(x => x.Id == request.OrganizationId)
            .Select(x => new OrganizationSettingsVM(x.Name, x.OwnerId))
            .FirstAsync();
    }
}
