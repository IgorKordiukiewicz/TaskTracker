using Domain.Users;

namespace Infrastructure.Repositories;

public class UserRepository : IRepository<User>
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetBy(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.Users
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task<bool> Exists(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbContext.Users.AnyAsync(predicate, cancellationToken);

    public async Task Add(User entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(User entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
