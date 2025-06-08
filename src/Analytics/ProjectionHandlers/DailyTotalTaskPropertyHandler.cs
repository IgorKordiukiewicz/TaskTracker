using Analytics.Infrastructure.Models;
using Analytics.Services;

namespace Analytics.ProjectionHandlers;

public abstract class DailyTotalTaskPropertyHandler<TProjection, TProperty>(IRepository repository, Func<TProjection, TProperty, bool> predicate)
    : ProjectionHandler<TProjection>(repository)
    where TProjection : class, IDailyCountProjection, new()
{
    protected void UpdateStatusCount(Guid projectId, TProperty property, DateTime date, bool increment = true)
    {
        var currentDayProjection = Find(x => x.ProjectId == projectId && x.Date.Date == date && predicate(x, property));
        var countChange = increment ? 1 : -1;

        if (currentDayProjection is null)
        {
            var previousDayProjection = GetPreviousDayProjection(projectId, property, date);
            var updatedCount = previousDayProjection is not null
                ? previousDayProjection.Count + countChange
                : (increment ? 1 : 0);

            Add(CreateProjection(projectId, date, property, updatedCount));
        }
        else
        {
            currentDayProjection.Count += countChange;
        }
    }

    protected abstract TProjection CreateProjection(Guid projectId, DateTime date, TProperty property, int count);

    private TProjection? GetPreviousDayProjection(Guid projectId, TProperty property, DateTime currentDate)
    {
        var previousDay = currentDate.AddDays(-1);
        return Find(x => x.ProjectId == projectId && x.Date == previousDay && predicate(x, property));
    }
}
