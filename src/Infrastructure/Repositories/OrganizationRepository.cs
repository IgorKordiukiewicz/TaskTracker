using Domain.Organizations;

namespace Infrastructure.Repositories;

public class OrganizationRepository : IRepository<Organization>
{
    private readonly AppDbContext _dbContext;

    public OrganizationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Organization?> GetBy(Expression<Func<Organization, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.Organizations
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Organization?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task<bool> Exists(Expression<Func<Organization, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.Organizations
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .AnyAsync(predicate, cancellationToken);

    public async Task Add(Organization entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Organizations.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Organization entity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbContext.Organizations
            .AsNoTracking()
            .Include(x => x.Members)
            .Include(x => x.Invitations)
            .Include(x => x.Roles)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);
        
        _dbContext.AddRemoveChildEntities(entity.Members, oldEntity.Members.Select(x => x.Id));
        _dbContext.AddRemoveChildEntities(entity.Invitations, oldEntity.Invitations.Select(x => x.Id));
        _dbContext.AddRemoveChildEntities(entity.Roles,oldEntity.Roles.Select(x => x.Id));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
