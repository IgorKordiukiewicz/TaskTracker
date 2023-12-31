﻿using Domain.Organizations;
using Shared.Enums;

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
        var organization = await _dbContext.Organizations
            .AsNoTracking()
            .Include(x => x.Invitations)
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == request.OrganizationId);
        if(organization is null)
        {
            return Result.Fail<UsersSearchVM>(new NotFoundError<Organization>(request.OrganizationId));
        }

        var membersUsers = organization.Members.Select(x => x.UserId);
        var usersWithPendingInvitations = organization.Invitations
            .Where(x => x.State == OrganizationInvitationState.Pending)
            .Select(x => x.UserId)
            .Distinct();
        var unavailableUsers = membersUsers.Concat(usersWithPendingInvitations);

        var searchValue = request.SearchValue.ToLower();
        var users = await _dbContext.Users
            .Where(x => !unavailableUsers.Contains(x.Id) && x.Email.ToLower().Contains(searchValue))
            .Take(5)
            .Select(x => new UserSearchVM
            {
                Id = x.Id,
                Email = x.Email,
            })
            .ToListAsync();

        return Result.Ok(new UsersSearchVM(users));
    }
}
