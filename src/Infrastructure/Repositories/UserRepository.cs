using Domain.Users;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) 
    : IRepository<User>
{
    public async Task<User?> GetBy(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Users
        .FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await GetBy(x => x.Id == id, cancellationToken);

    public async Task<bool> Exists(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => await dbContext.Users.AnyAsync(predicate, cancellationToken);

    public async Task Add(User entity, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(User entity, CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
