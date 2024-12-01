using Domain.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Repositories;

public class TaskRelationshipManagerRepository : IRepository<TaskRelationshipManager>
{
    private readonly AppDbContext _dbContext;

    public TaskRelationshipManagerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(TaskRelationshipManager entity, CancellationToken cancellationToken = default)
    {
        _dbContext.TaskRelationshipManagers.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<TaskRelationshipManager, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.TaskRelationshipManagers
        .AnyAsync(predicate, cancellationToken);

    public async Task<TaskRelationshipManager?> GetBy(Expression<Func<TaskRelationshipManager, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.TaskRelationshipManagers
        .Include(x => x.HierarchicalRelationships)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<TaskRelationshipManager?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task Update(TaskRelationshipManager entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbContext.TaskRelationshipManagers
            .AsNoTracking()
            .Include(x => x.HierarchicalRelationships)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);

        _dbContext.AddRemoveChildValueObjects(entity.HierarchicalRelationships, oldEntity.HierarchicalRelationships);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
