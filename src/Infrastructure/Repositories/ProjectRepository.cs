using Domain.Projects;

namespace Infrastructure.Repositories;

public class ProjectRepository(AppDbContext dbContext) 
    : IRepository<Project>
{
    public async Task<Project?> GetBy(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Projects
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Project?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task<bool> Exists(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Projects
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .AnyAsync(predicate, cancellationToken);

    public async Task Add(Project entity, CancellationToken cancellationToken = default)
    {
        dbContext.Projects.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Project entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await dbContext.Projects
            .AsNoTracking()
            .Include(x => x.Members)
            .Include(x => x.Invitations)
            .Include(x => x.Roles)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);
        
        dbContext.AddRemoveChildEntities(entity.Members, oldEntity.Members.Select(x => x.Id));
        dbContext.AddRemoveChildEntities(entity.Invitations, oldEntity.Invitations.Select(x => x.Id));
        dbContext.AddRemoveChildEntities(entity.Roles,oldEntity.Roles.Select(x => x.Id));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
