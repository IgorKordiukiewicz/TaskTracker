using Analytics.Infrastructure;
using Analytics.ViewModels;

namespace Analytics.Services;

public interface IQueryService
{
    Task<TotalTaskStatusesVM> GetTotalTaskStatuses(Guid projectId);
}

public class QueryService(AnalyticsDbContext dbContext) : IQueryService
{
    public async Task<TotalTaskStatusesVM> GetTotalTaskStatuses(Guid projectId) // TODO: rename to current statuses or sth?
    {
        var today = DateTime.UtcNow.Date; // TODO: DateTimeProvider?
        var countByStatusId = await dbContext.DailyTotalTaskStatuses
            .Where(x => x.ProjectId == projectId && x.Date == today)
            .ToDictionaryAsync(k => k.StatusId, v => v.Count);
        return new(countByStatusId);
    }
}
