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

    public async Task Add(TaskRelationshipManager entity)
    {
        _dbContext.TaskRelationshipManagers.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> Exists(Expression<Func<TaskRelationshipManager, bool>> predicate)
        => await _dbContext.TaskRelationshipManagers
        .AnyAsync(predicate);

    public async Task<TaskRelationshipManager?> GetBy(Expression<Func<TaskRelationshipManager, bool>> predicate)
        => await _dbContext.TaskRelationshipManagers
        .Include(x => x.HierarchicalRelationships)
        .FirstOrDefaultAsync(predicate);

    public async Task<TaskRelationshipManager?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task Update(TaskRelationshipManager entity)
    {
        var oldEntity = await _dbContext.TaskRelationshipManagers
            .AsNoTracking()
            .Include(x => x.HierarchicalRelationships)
            .SingleAsync(x => x.Id == entity.Id);

        _dbContext.AddRemoveChildValueObjects(entity.HierarchicalRelationships, oldEntity.HierarchicalRelationships);
        await _dbContext.SaveChangesAsync();
    }
}
