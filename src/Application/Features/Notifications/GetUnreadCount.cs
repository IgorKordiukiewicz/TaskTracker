﻿namespace Application.Features.Notifications;

public record GetUnreadNotificationsCountQuery(Guid UserId) : IRequest<int>;

internal class GetUnreadNotificationsCountHandler(AppDbContext dbContext)
    : IRequestHandler<GetUnreadNotificationsCountQuery, int>
{
    public async Task<int> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        var notificationsCount = await dbContext.Notifications
            .AsNoTracking()
            .CountAsync(x => x.UserId == request.UserId && !x.Read, cancellationToken);
        var pendingInvitationsCount = await dbContext.OrganizationInvitations
            .AsNoTracking()
            .CountAsync(x => x.UserId == request.UserId && x.State == OrganizationInvitationState.Pending, cancellationToken);
        return notificationsCount + pendingInvitationsCount;
    }
}
