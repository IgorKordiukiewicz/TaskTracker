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
            var lastProjection = GetLastProjection(projectId, property, date);
            var updatedCount = lastProjection is not null
                ? lastProjection.Count + countChange
                : (increment ? 1 : 0);

            Add(CreateProjection(projectId, date, property, updatedCount));
        }
        else
        {
            currentDayProjection.Count += countChange;
        }
    }

    protected abstract TProjection CreateProjection(Guid projectId, DateTime date, TProperty property, int count);

    private TProjection? GetLastProjection(Guid projectId, TProperty property, DateTime currentDate)
    {
        return Projections
            .OrderByDescending(x => x.Date)
            .FirstOrDefault(x => x.ProjectId == projectId && x.Date < currentDate && predicate(x, property));
    }
}
