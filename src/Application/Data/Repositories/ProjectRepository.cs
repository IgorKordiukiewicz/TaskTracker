using Domain.Projects;
using System.Linq.Expressions;

namespace Application.Data.Repositories;

public class ProjectRepository : IRepository<Project>
{
    private readonly AppDbContext _dbContext;

    public ProjectRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Project?> GetBy(Expression<Func<Project, bool>> predicate)
        => await _dbContext.Projects
        .Include(x => x.Members)
        .FirstOrDefaultAsync(predicate);

    public async Task<Project?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task<bool> Exists(Expression<Func<Project, bool>> predicate)
        => await _dbContext.Projects
        .Include(x => x.Members)
        .AnyAsync(predicate);

    public async Task Add(Project entity)
    {
        _dbContext.Projects.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Project entity)
    {
        await _dbContext.AddRemoveDependents(entity.Members);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Project entity)
    {
        _dbContext.Projects.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}
