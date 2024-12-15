using Domain.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Repositories;

public class TaskRelationshipManagerRepository(AppDbContext dbContext) 
    : IRepository<TaskRelationshipManager>
{
    public async Task Add(TaskRelationshipManager entity, CancellationToken cancellationToken = default)
    {
        dbContext.TaskRelationshipManagers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<TaskRelationshipManager, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.TaskRelationshipManagers
        .AnyAsync(predicate, cancellationToken);

    public async Task<TaskRelationshipManager?> GetBy(Expression<Func<TaskRelationshipManager, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.TaskRelationshipManagers
        .Include(x => x.HierarchicalRelationships)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<TaskRelationshipManager?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task Update(TaskRelationshipManager entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await dbContext.TaskRelationshipManagers
            .AsNoTracking()
            .Include(x => x.HierarchicalRelationships)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);

        dbContext.AddRemoveChildValueObjects(entity.HierarchicalRelationships, oldEntity.HierarchicalRelationships);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
