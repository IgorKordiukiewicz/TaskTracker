namespace Infrastructure.Repositories;

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

    public async Task<bool> Exists(Expression<Func<Domain.Tasks.Task, bool>> predicate)
        => await _dbContext.Tasks
        .Include(x => x.Comments)
        .Include(x => x.Activities)
        .Include(x => x.TimeLogs)
        .AnyAsync(predicate);

    public async Task<Domain.Tasks.Task?> GetBy(Expression<Func<Domain.Tasks.Task, bool>> predicate)
        => await _dbContext.Tasks
        .Include(x => x.Comments)
        .Include(x => x.Activities)
        .Include(x => x.TimeLogs)
        .FirstOrDefaultAsync(predicate);

    public async Task<Domain.Tasks.Task?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task Update(Domain.Tasks.Task entity)
    {
        var oldEntity = await _dbContext.Tasks
            .AsNoTracking()
            .Include(x => x.Comments)
            .Include(x => x.Activities)
            .Include(x => x.TimeLogs)
            .SingleAsync(x => x.Id == entity.Id);
        
        _dbContext.AddRemoveChildEntities(entity.Comments,oldEntity.Comments.Select(x => x.Id));
        _dbContext.AddRemoveChildValueObjects(entity.Activities, oldEntity.Activities);
        _dbContext.AddRemoveChildValueObjects(entity.TimeLogs, oldEntity.TimeLogs);
        await _dbContext.SaveChangesAsync();
    }
}
