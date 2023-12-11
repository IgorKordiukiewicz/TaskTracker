using Domain.Projects;
using Domain.Users;
using System.Linq.Expressions;

namespace Application.Data.Repositories;

public class UserRepository : IRepository<User>
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetBy(Expression<Func<User, bool>> predicate)
        => await _dbContext.Users
        .FirstOrDefaultAsync(predicate);

    public async Task<User?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task<bool> Exists(Expression<Func<User, bool>> predicate)
        => await _dbContext.Users.AnyAsync(predicate);

    public async Task Add(User entity)
    {
        _dbContext.Users.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(User entity)
    {
        await _dbContext.SaveChangesAsync();
    }
}
