using Domain.Notifications;

namespace Infrastructure.Repositories;

public class NotificationRepository(AppDbContext dbContext)
    : IRepository<Notification>
{
    public async Task Add(Notification entity, CancellationToken cancellationToken = default)
    {
        dbContext.Notifications.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<Notification, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Notifications.AnyAsync(predicate, cancellationToken);

    public async Task<Notification?> GetBy(Expression<Func<Notification, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Notifications.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Notification?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task Update(Notification entity, CancellationToken cancellationToken = default)
    {
        dbContext.Notifications.Update(entity);
        await dbContext.SaveChangesAsync();
    }
}
