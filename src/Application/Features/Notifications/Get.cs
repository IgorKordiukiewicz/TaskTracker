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

        var entityNamesByIdByContext = new Dictionary<NotificationContext, IReadOnlyDictionary<Guid, string>>()
        {
            { NotificationContext.Organization, await GetEntityNameById(notifications, NotificationContext.Organization, dbContext.Organizations) },
            { NotificationContext.Project, await GetEntityNameById(notifications, NotificationContext.Project, dbContext.Projects) },
        };

        return new(notifications.Select(x => new NotificationVM(x.Id, x.Message, x.OccurredAt, x.Context, x.ContextEntityId, 
            ResolveEntitiesNames(entityNamesByIdByContext, x.Context, x.ContextEntityId), x.TaskShortId)).ToList());
    }

    private static async Task<IReadOnlyDictionary<Guid, string>> GetEntityNameById<TEntity>(List<Notification> notifications, NotificationContext context, DbSet<TEntity> entities)
        where TEntity : Entity, IHasName
    {
        var ids = notifications
            .Where(x => x.Context == context)
            .Select(x => x.ContextEntityId);

        return await entities
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(k => k.Id, v => v.Name);
    }

    private static string ResolveEntitiesNames(Dictionary<NotificationContext, IReadOnlyDictionary<Guid, string>> entityNamesByIdByContext, NotificationContext context, Guid id)
    {
        var entityNamesById = entityNamesByIdByContext[context];
        return entityNamesById.TryGetValue(id, out var name) ? name : string.Empty;
    }
}
