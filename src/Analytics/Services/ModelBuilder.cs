using Analytics.Infrastructure.Models;

namespace Analytics.Services;

public static class ModelBuilder
{
    public static (IReadOnlyList<DateTime> Dates, IReadOnlyDictionary<TProperty, IReadOnlyList<int>> DailyCountByProperty) BuildTotalTaskPropertiesByDay<TProjection, TProperty>(
        IReadOnlyList<TProjection> dailyTotalProperties, 
        Func<TProjection, TProperty> propertySelector, 
        Func<TProjection, TProperty, bool> propertyQuery)
        where TProjection : IDailyCountProjection
        where TProperty : notnull
    {
        if(dailyTotalProperties.Count == 0)
        {
            return (new List<DateTime>(), new Dictionary<TProperty, IReadOnlyList<int>>());
        }

        var firstDate = dailyTotalProperties.Min(x => x.Date);
        var lastDate = dailyTotalProperties.Max(x => x.Date);
        var allDates = Enumerable.Range(0, (lastDate - firstDate).Days + 1)
            .Select(x => firstDate.AddDays(x))
            .ToList();

        var allProperties = dailyTotalProperties
            .Select(x => propertySelector(x))
            .Distinct()
            .ToList();

        var result = new Dictionary<TProperty, IReadOnlyList<int>>();
        foreach (var property in allProperties)
        {
            var counts = new List<int>();
            var projections = dailyTotalProperties
                .Where(x => propertyQuery(x, property))
                .OrderBy(x => x.Date)
                .ToList();
            var projectionIndex = 0;
            var lastCount = 0;
            foreach (var date in allDates)
            {
                var projectionDate = projections[projectionIndex].Date;

                // no projection for this date, apply last known count
                if (projectionDate > date)
                {
                    counts.Add(lastCount);
                }
                // matching dates, apply projection's count
                else if (projectionDate == date)
                {
                    lastCount = projections[projectionIndex].Count;
                    counts.Add(lastCount);
                    ++projectionIndex;
                }

                if(projectionIndex == projections.Count)
                {
                    // no more projections, fill the rest with last known count
                    counts.AddRange(Enumerable.Repeat(lastCount, allDates.Count - counts.Count));
                    break;
                }
            }

            result.Add(property, counts);
        }

        return new(allDates, result);
    }
}
