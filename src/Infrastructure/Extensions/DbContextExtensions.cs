using FluentResults;
using Infrastructure.Errors;

namespace Infrastructure.Extensions;

public static class DbContextExtensions
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
}
