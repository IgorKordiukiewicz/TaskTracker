using Analytics.Infrastructure.Models;
using Analytics.ViewModels;

namespace Analytics.Services;

public static class ModelBuilder
{
    public static TotalTaskStatusesByDayVM BuildTotalTaskStatusesByDay(IReadOnlyList<DailyTotalTaskStatus> dailyTotalTaskStatuses)
    {
        var firstDate = dailyTotalTaskStatuses.Min(x => x.Date);
        var lastDate = dailyTotalTaskStatuses.Max(x => x.Date);
        var allDates = Enumerable.Range(0, (lastDate - firstDate).Days + 1)
            .Select(x => firstDate.AddDays(x))
            .ToList();

        var allStatuses = dailyTotalTaskStatuses
            .Select(x => x.StatusId)
            .Distinct()
            .ToList();

        var result = new Dictionary<Guid, IReadOnlyList<int>>();
        foreach (var status in allStatuses)
        {
            var counts = new List<int>();
            var projections = dailyTotalTaskStatuses
                .Where(x => x.StatusId == status)
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

            result.Add(status, counts);
        }

        return new(allDates, result);
    }
}
