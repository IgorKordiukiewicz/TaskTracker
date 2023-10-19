using Domain.Workflows;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace Application.Data.Repositories;

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

    public async Task Delete(Workflow entity)
    {
        _dbContext.Workflows.Remove(entity);
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
        await _dbContext.AddRemoveDependentEntities(entity.Statuses);
        await _dbContext.AddRemoveDependentValueObjects(entity.Transitions);
        await _dbContext.SaveChangesAsync();
    }
}
