using Domain.Common;
using Domain.Projects;
using System.Linq.Expressions;

namespace Application.Data.Repositories;

public interface IRepository<TEntity> 
    where TEntity : Entity 
{
    Task<TEntity?> GetById(Guid id); // TODO: Remove?
    Task<TEntity?> GetBy(Expression<Func<TEntity, bool>> predicate);
    Task<bool> Exists(Expression<Func<TEntity, bool>> predicate);

    Task Add(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
}