using Domain.Organizations;

namespace Infrastructure.Repositories;

public class OrganizationRepository : IRepository<Organization>
{
    private readonly AppDbContext _dbContext;

    public OrganizationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Organization?> GetBy(Expression<Func<Organization, bool>> predicate)
        => await _dbContext.Organizations
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .FirstOrDefaultAsync(predicate);

    public async Task<Organization?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task<bool> Exists(Expression<Func<Organization, bool>> predicate)
        => await _dbContext.Organizations
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .AnyAsync(predicate);

    public async Task Add(Organization entity)
    {
        _dbContext.Organizations.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Organization entity)
    {
        var oldEntity = await _dbContext.Organizations
            .AsNoTracking()
            .Include(x => x.Members)
            .Include(x => x.Invitations)
            .Include(x => x.Roles)
            .SingleAsync(x => x.Id == entity.Id);
        
        _dbContext.AddRemoveChildEntities(entity.Members, oldEntity.Members.Select(x => x.Id));
        _dbContext.AddRemoveChildEntities(entity.Invitations, oldEntity.Invitations.Select(x => x.Id));
        _dbContext.AddRemoveChildEntities(entity.Roles,oldEntity.Roles.Select(x => x.Id));
        await _dbContext.SaveChangesAsync();
    }
}
