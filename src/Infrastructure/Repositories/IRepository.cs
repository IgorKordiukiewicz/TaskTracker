using Domain.Common;

namespace Infrastructure.Repositories;

public interface IRepository<TEntity> 
    where TEntity : Entity 
{
    Task<TEntity?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetBy(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task Add(TEntity entity, CancellationToken cancellationToken = default);
    Task Update(TEntity entity, CancellationToken cancellationToken = default);
}