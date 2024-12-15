using Domain.Workflows;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Repositories;

public class WorkflowRepository(AppDbContext dbContext) 
    : IRepository<Workflow>
{
    public async Task Add(Workflow entity, CancellationToken cancellationToken = default)
    {
        dbContext.Workflows.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<Workflow, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Workflows
        .AnyAsync(predicate, cancellationToken);

    public async Task<Workflow?> GetBy(Expression<Func<Workflow, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Workflows
        .Include(x => x.Statuses)
        .Include(x => x.Transitions)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Workflow?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task Update(Workflow entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await dbContext.Workflows
            .AsNoTracking()
            .Include(x => x.Statuses)
            .Include(x => x.Transitions)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);
        
        dbContext.AddRemoveChildEntities(entity.Statuses, oldEntity.Statuses.Select(x => x.Id));
        dbContext.AddRemoveChildValueObjects(entity.Transitions, oldEntity.Transitions);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
