using System.Linq.Expressions;

namespace Application.Data.Repositories;

public class TaskRepository : IRepository<Domain.Tasks.Task>
{
    private readonly AppDbContext _dbContext;

    public TaskRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Domain.Tasks.Task entity)
    {
        _dbContext.Tasks.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Domain.Tasks.Task entity)
    {
        _dbContext.Tasks.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> Exists(Expression<Func<Domain.Tasks.Task, bool>> predicate)
        => await _dbContext.Tasks
        .Include(x => x.Comments)
        .AnyAsync(predicate);

    public async Task<Domain.Tasks.Task?> GetBy(Expression<Func<Domain.Tasks.Task, bool>> predicate)
        => await _dbContext.Tasks
        .Include(x => x.Comments)
        .FirstOrDefaultAsync(predicate);

    public async Task<Domain.Tasks.Task?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task Update(Domain.Tasks.Task entity)
    {
        await _dbContext.AddRemoveChildEntities(entity.Comments);
        await _dbContext.SaveChangesAsync();
    }
}
