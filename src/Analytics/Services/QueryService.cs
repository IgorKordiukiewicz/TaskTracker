using Analytics.Infrastructure;
using Analytics.ViewModels;

namespace Analytics.Services;

public interface IQueryService
{
    Task<TotalTaskStatusesVM> GetTotalTaskStatuses(Guid projectId);
    Task<TotalTaskStatusesByDayVM> GetTotalTaskStatusesByDay(Guid projectId);
}

public class QueryService(AnalyticsDbContext dbContext) : IQueryService
{
    public async Task<TotalTaskStatusesVM> GetTotalTaskStatuses(Guid projectId) // TODO: rename to current statuses or sth?
    {
        var today = DateTime.UtcNow.Date; // TODO: DateTimeProvider?
        var latestTotals = await dbContext.DailyTotalTaskStatuses
            .Where(x => x.ProjectId == projectId)
            .GroupBy(x => x.StatusId)
            .Select(x => x
                .OrderByDescending(xx => xx.Date)
                .FirstOrDefault()
            )
            .ToListAsync();

        return new(latestTotals
            .Where(x => x is not null)
            .ToDictionary(x => x!.StatusId, x => x!.Count));
    }

    public async Task<TotalTaskStatusesByDayVM> GetTotalTaskStatusesByDay(Guid projectId)
    {
        var dailyTotals = await dbContext.DailyTotalTaskStatuses
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
        return ModelBuilder.BuildTotalTaskStatusesByDay(dailyTotals);
    }
}
