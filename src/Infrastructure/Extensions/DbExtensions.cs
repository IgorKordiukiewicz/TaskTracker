using FluentResults;
using Infrastructure.Errors;

namespace Infrastructure.Extensions;

public static class DbExtensions
{
    public static async Task<Result> ExecuteTransaction(this AppDbContext dbContext, Func<Task> action)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await action();

            await transaction.CommitAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new InfrastructureError("SQL Transaction failure").CausedBy(ex));
        }
    }

    public static async Task DeleteAll<TEntity>(this DbSet<TEntity> entities, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        await entities
            .Where(predicate)
            .ExecuteUpdateAsync(x => x.SetProperty(p => EF.Property<bool>(p, "IsDeleted"), true));
    }
}
