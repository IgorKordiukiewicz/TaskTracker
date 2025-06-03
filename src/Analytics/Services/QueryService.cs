using Analytics.Infrastructure;
using Analytics.Infrastructure.Models;
using Analytics.ViewModels;

namespace Analytics.Services;

public interface IQueryService
{
    Task<TotalTaskStatusesVM> GetTotalTaskStatuses(Guid projectId);
    Task<TotalTaskStatusesByDayVM> GetTotalTaskStatusesByDay(Guid projectId);
    Task<TotalTaskPrioritiesVM> GetTotalTaskPriorites(Guid projectId);
    Task<TotalTaskPrioritiesByDayVM> GetTotalTaskPrioritiesByDay(Guid projectId);
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
        var (dates, countsByProperty) = ModelBuilder.BuildTotalTaskStatusesByDay(dailyTotals, 
            x => x.StatusId, 
            (x, property) => x.StatusId == property);
        return new(dates, countsByProperty);
    }

    public async Task<TotalTaskPrioritiesVM> GetTotalTaskPriorites(Guid projectId)
    {
        var today = DateTime.UtcNow.Date;
        var latestTotals = await dbContext.DailyTotalTaskPriorities
            .Where(x => x.ProjectId == projectId)
            .GroupBy(x => x.Priority)
            .Select(x => x
                .OrderByDescending(xx => xx.Date)
                .FirstOrDefault()
            )
            .ToListAsync();

        return new(latestTotals
            .Where(x => x is not null)
            .ToDictionary(x => x!.Priority, x => x!.Count));
    }

    public async Task<TotalTaskPrioritiesByDayVM> GetTotalTaskPrioritiesByDay(Guid projectId)
    {
        var dailyTotals = await dbContext.DailyTotalTaskPriorities
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
        var (dates, countsByProperty) = ModelBuilder.BuildTotalTaskStatusesByDay(dailyTotals,
            x => x.Priority,
            (x, property) => x.Priority == property);
        return new(dates, countsByProperty);
    }
}
