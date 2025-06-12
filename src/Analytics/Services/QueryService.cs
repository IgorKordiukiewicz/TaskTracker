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
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
        var dailyTotalProrities = await dbContext.DailyTotalTaskPriorities
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
        var dailyTotalAssignees = await dbContext.DailyTotalTaskAssignees
            .AsNoTracking()
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

        var countByAssigneeId = dailyTotalAssignees
            .GroupBy(x => x.AssigneeId)
            .Select(x => x
                .OrderByDescending(xx => xx.Date)
                .FirstOrDefault()
            )
            .Where(x => x is not null)
            .ToDictionary(k => k!.AssigneeId, v => v!.Count);

        var (dates, dailyCountByStatusId) = ModelBuilder.BuildTotalTaskPropertiesByDay(dailyTotalStatuses,
            x => x.StatusId,
            (x, property) => x.StatusId == property);

        var (_, dailyCountByPriority) = ModelBuilder.BuildTotalTaskPropertiesByDay(dailyTotalProrities,
            x => x.Priority,
            (x, property) => x.Priority == property);

        var (_, dailyCountByAssignee) = ModelBuilder.BuildTotalTaskPropertiesByDay(dailyTotalAssignees,
            x => x.AssigneeId,
            (x, property) => x.AssigneeId == property);

        return new()
        {
            Dates = dates,

            CountByPriority = countByPriority,
            CountByStatusId = countByStatusId,
            CountByAssigneeId = countByAssigneeId,

            DailyCountByStatusId = dailyCountByStatusId,
            DailyCountByPriority = dailyCountByPriority,
            DailyCountByAssigneeId = dailyCountByAssignee
        };
    }
}
