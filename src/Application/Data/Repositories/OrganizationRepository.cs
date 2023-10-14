using Domain.Organizations;
using Domain.Projects;
using System.Linq.Expressions;

namespace Application.Data.Repositories;

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
        .FirstOrDefaultAsync(predicate);

    public async Task<Organization?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task<bool> Exists(Expression<Func<Organization, bool>> predicate)
        => await _dbContext.Organizations
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .AnyAsync(predicate);

    public async Task Add(Organization entity)
    {
        _dbContext.Organizations.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Organization entity)
    {
        await _dbContext.AddRemoveDependentEntities(entity.Members);
        await _dbContext.AddRemoveDependentEntities(entity.Invitations);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Organization entity)
    {
        _dbContext.Organizations.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}
