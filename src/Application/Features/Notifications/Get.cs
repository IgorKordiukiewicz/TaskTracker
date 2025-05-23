using Domain.Common;
using Domain.Notifications;

namespace Application.Features.Notifications;

public record GetNotificationsQuery(Guid UserId) : IRequest<NotificationsVM>;

internal class GetNotificationsHandler(AppDbContext dbContext)
    : IRequestHandler<GetNotificationsQuery, NotificationsVM>
{
    public async Task<NotificationsVM> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await dbContext.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && !x.Read)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync(cancellationToken);

        var projectIds = notifications
            .Select(r => r.ContextEntityId);

        var projectNameById = await dbContext.Projects
            .Where(x => projectIds.Contains(x.Id))
            .ToDictionaryAsync(k => k.Id, v => v.Name, cancellationToken);

        return new(notifications
            .Select(x => new NotificationVM(x.Id, x.Message, x.OccurredAt, x.ContextEntityId, projectNameById.GetValueOrDefault(x.ContextEntityId) ?? string.Empty, x.TaskShortId))
            .ToList());
    }
}
