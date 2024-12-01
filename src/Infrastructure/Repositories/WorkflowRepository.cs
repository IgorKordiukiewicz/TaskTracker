using Domain.Workflows;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Repositories;

public class WorkflowRepository : IRepository<Workflow>
{
    private readonly AppDbContext _dbContext;

    public WorkflowRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Workflow entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Workflows.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<Workflow, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.Workflows
        .AnyAsync(predicate, cancellationToken);

    public async Task<Workflow?> GetBy(Expression<Func<Workflow, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.Workflows
        .Include(x => x.Statuses)
        .Include(x => x.Transitions)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Workflow?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task Update(Workflow entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbContext.Workflows
            .AsNoTracking()
            .Include(x => x.Statuses)
            .Include(x => x.Transitions)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);
        
        _dbContext.AddRemoveChildEntities(entity.Statuses, oldEntity.Statuses.Select(x => x.Id));
        _dbContext.AddRemoveChildValueObjects(entity.Transitions, oldEntity.Transitions);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
