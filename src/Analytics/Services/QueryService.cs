using Analytics.Infrastructure;
using Analytics.ViewModels;

namespace Analytics.Services;

public interface IQueryService
{
    Task<TaskAnalyticsVM> GetTaskAnalytics(Guid projectId);
}

public class QueryService(AnalyticsDbContext dbContext) : IQueryService
{
    public async Task<TaskAnalyticsVM> GetTaskAnalytics(Guid projectId)
    {
        var dailyTotalStatuses = await dbContext.DailyTotalTaskStatuses
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
        var dailyTotalProrities = await dbContext.DailyTotalTaskPriorities
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();

        var countByStatusId = dailyTotalStatuses
            .GroupBy(x => x.StatusId)
            .Select(x => x
                .OrderByDescending(xx => xx.Date)
                .FirstOrDefault()
            )
            .Where(x => x is not null)
            .ToDictionary(k => k!.StatusId, v => v!.Count);

        var countByPriority = dailyTotalProrities
            .GroupBy(x => x.Priority)
            .Select(x => x
                .OrderByDescending(xx => xx.Date)
                .FirstOrDefault()
            )
            .Where(x => x is not null)
            .ToDictionary(k => k!.Priority, v => v!.Count);

        var (dates, dailyCountByStatusId) = ModelBuilder.BuildTotalTaskPropertiesByDay(dailyTotalStatuses,
            x => x.StatusId,
            (x, property) => x.StatusId == property);

        var (_, dailyCountByPriority) = ModelBuilder.BuildTotalTaskPropertiesByDay(dailyTotalProrities,
            x => x.Priority,
            (x, property) => x.Priority == property);

        return new()
        {
            Dates = dates,

            CountByPriority = countByPriority,
            CountByStatusId = countByStatusId,

            DailyCountByStatusId = dailyCountByStatusId,
            DailyCountByPriority = dailyCountByPriority
        };
    }
}
