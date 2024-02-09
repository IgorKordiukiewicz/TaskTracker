using Domain.Common;

namespace Infrastructure.Repositories;

public interface IRepository<TEntity> 
    where TEntity : Entity 
{
    Task<TEntity?> GetById(Guid id);
    Task<TEntity?> GetBy(Expression<Func<TEntity, bool>> predicate);
    Task<bool> Exists(Expression<Func<TEntity, bool>> predicate);

    Task Add(TEntity entity);
    Task Update(TEntity entity);
}