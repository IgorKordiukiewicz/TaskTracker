namespace Infrastructure.Repositories;

public class TaskRepository(AppDbContext dbContext) 
    : IRepository<Domain.Tasks.Task>
{
    public async Task Add(Domain.Tasks.Task entity, CancellationToken cancellationToken = default)
    {
        dbContext.Tasks.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<Domain.Tasks.Task, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Tasks
        .Include(x => x.Comments)
        .Include(x => x.Activities)
        .Include(x => x.TimeLogs)
        .AnyAsync(predicate, cancellationToken);

    public async Task<Domain.Tasks.Task?> GetBy(Expression<Func<Domain.Tasks.Task, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Tasks
        .Include(x => x.Comments)
        .Include(x => x.Activities)
        .Include(x => x.TimeLogs)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Domain.Tasks.Task?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task Update(Domain.Tasks.Task entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await dbContext.Tasks
            .AsNoTracking()
            .Include(x => x.Comments)
            .Include(x => x.Activities)
            .Include(x => x.TimeLogs)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);
        
        dbContext.AddRemoveChildEntities(entity.Comments,oldEntity.Comments.Select(x => x.Id));
        dbContext.AddRemoveChildValueObjects(entity.Activities, oldEntity.Activities);
        dbContext.AddRemoveChildValueObjects(entity.TimeLogs, oldEntity.TimeLogs);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
