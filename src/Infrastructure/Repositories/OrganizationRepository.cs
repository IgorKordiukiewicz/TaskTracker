using Domain.Organizations;

namespace Infrastructure.Repositories;

public class OrganizationRepository(AppDbContext dbContext) 
    : IRepository<Organization>
{
    public async Task<Organization?> GetBy(Expression<Func<Organization, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Organizations
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Organization?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task<bool> Exists(Expression<Func<Organization, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Organizations
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .AnyAsync(predicate, cancellationToken);

    public async Task Add(Organization entity, CancellationToken cancellationToken = default)
    {
        dbContext.Organizations.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Organization entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await dbContext.Organizations
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
