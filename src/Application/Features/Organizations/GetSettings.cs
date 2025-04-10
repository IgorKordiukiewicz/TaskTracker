﻿using Domain.Organizations;

namespace Application.Features.Organizations;

public record GetOrganizationSettingsQuery(Guid OrganizationId) : IRequest<Result<OrganizationSettingsVM>>;

internal class GetOrganizationSettingsQueryValidator : AbstractValidator<GetOrganizationSettingsQuery>
{
    public GetOrganizationSettingsQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
    }
}

internal class GetOrganizationSettingsHandler(AppDbContext dbContext) 
    : IRequestHandler<GetOrganizationSettingsQuery, Result<OrganizationSettingsVM>>
{
    public async Task<Result<OrganizationSettingsVM>> Handle(GetOrganizationSettingsQuery request, CancellationToken cancellationToken)
    {
        if(!await dbContext.Organizations.AnyAsync(x => x.Id == request.OrganizationId, cancellationToken))
        {
            return Result.Fail<OrganizationSettingsVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        return await dbContext.Organizations
            .Where(x => x.Id == request.OrganizationId)
            .Select(x => new OrganizationSettingsVM(x.Name, x.OwnerId))
            .FirstAsync(cancellationToken);
    }
}
