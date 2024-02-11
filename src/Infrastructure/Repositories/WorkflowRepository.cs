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

    public async Task Add(Workflow entity)
    {
        _dbContext.Workflows.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> Exists(Expression<Func<Workflow, bool>> predicate)
        => await _dbContext.Workflows
        .AnyAsync(predicate);

    public async Task<Workflow?> GetBy(Expression<Func<Workflow, bool>> predicate)
        => await _dbContext.Workflows
        .Include(x => x.Statuses)
        .Include(x => x.Transitions)
        .FirstOrDefaultAsync(predicate);

    public async Task<Workflow?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task Update(Workflow entity)
    {
        var oldEntity = await _dbContext.Workflows
            .AsNoTracking()
            .Include(x => x.Statuses)
            .Include(x => x.Transitions)
            .SingleAsync(x => x.Id == entity.Id);
        
        _dbContext.AddRemoveChildEntities(entity.Statuses, oldEntity.Statuses.Select(x => x.Id));
        _dbContext.AddRemoveChildValueObjects(entity.Transitions, oldEntity.Transitions);
        await _dbContext.SaveChangesAsync();
    }
}
