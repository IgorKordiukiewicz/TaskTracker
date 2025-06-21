using Domain.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Repositories;

public class TaskRelationManagerRepository(AppDbContext dbContext) 
    : IRepository<TaskRelationManager>
{
    public async Task Add(TaskRelationManager entity, CancellationToken cancellationToken = default)
    {
        dbContext.TaskRelationManagers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<TaskRelationManager, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.TaskRelationManagers
        .AnyAsync(predicate, cancellationToken);

    public async Task<TaskRelationManager?> GetBy(Expression<Func<TaskRelationManager, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.TaskRelationManagers
        .Include(x => x.HierarchicalRelations)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<TaskRelationManager?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task Update(TaskRelationManager entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await dbContext.TaskRelationManagers
            .AsNoTracking()
            .Include(x => x.HierarchicalRelations)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);

        dbContext.AddRemoveChildValueObjects(entity.HierarchicalRelations, oldEntity.HierarchicalRelations);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
