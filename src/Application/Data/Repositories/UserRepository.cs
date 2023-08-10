using Domain.Projects;
using Domain.Users;
using System.Linq.Expressions;

namespace Application.Data.Repositories;

// TODO: Add some common class for repositories since much of the code is almost the same?
// E.g. Repository<User> that accepts a list of dependent properties selectors (e.g. x => x.Members) (for Update)
// and static factory that returns those repositories for each AggregateRoot type
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

    public async Task Delete(User entity)
    {
        _dbContext.Users.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}
