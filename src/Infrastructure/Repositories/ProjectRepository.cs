using Domain.Projects;

namespace Infrastructure.Repositories;

public class ProjectRepository : IRepository<Project>
{
    private readonly AppDbContext _dbContext;

    public ProjectRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Project?> GetBy(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.Projects
        .Include(x => x.Members)
        .Include(x => x.Roles)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Project?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task<bool> Exists(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.Projects
        .Include(x => x.Members)
        .Include(x => x.Roles)
        .AnyAsync(predicate, cancellationToken);

    public async Task Add(Project entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Projects.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Project entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbContext.Projects
            .AsNoTracking()
            .Include(x => x.Members)
            .Include(x => x.Roles)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);
        
        _dbContext.AddRemoveChildEntities(entity.Members, oldEntity.Members.Select(x => x.Id));
        _dbContext.AddRemoveChildEntities(entity.Roles,oldEntity.Roles.Select(x => x.Id));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
