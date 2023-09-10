using Domain.Tasks;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace Application.Data.Repositories;

public class TaskStatesManagerRepository : IRepository<TaskStatesManager>
{
    private readonly AppDbContext _dbContext;

    public TaskStatesManagerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(TaskStatesManager entity)
    {
        _dbContext.TaskStatesManagers.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(TaskStatesManager entity)
    {
        _dbContext.TaskStatesManagers.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> Exists(Expression<Func<TaskStatesManager, bool>> predicate)
        => await _dbContext.TaskStatesManagers
        .AnyAsync(predicate);

    public async Task<TaskStatesManager?> GetBy(Expression<Func<TaskStatesManager, bool>> predicate)
        => await _dbContext.TaskStatesManagers
        .Include(x => x.AllStates)
        .FirstOrDefaultAsync(predicate);

    public async Task<TaskStatesManager?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task Update(TaskStatesManager entity)
    {
        await _dbContext.SaveChangesAsync();
    }
}
