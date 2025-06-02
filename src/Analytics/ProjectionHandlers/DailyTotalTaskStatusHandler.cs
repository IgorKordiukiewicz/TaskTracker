using Analytics.Infrastructure.Models;
using Analytics.Services;
using Domain.Common;
using Domain.Events;

namespace Analytics.ProjectionHandlers;

public class DailyTotalTaskStatusHandler(IRepository repository)
    : ProjectionHandler<DailyTotalTaskStatus>(repository)
{
    public override void ApplyEvent(DomainEvent domainEvent)
    {
        if (domainEvent is TaskCreated taskCreated)
        {
            IncrementStatusCount(taskCreated.ProjectId, taskCreated.StatusId, taskCreated.OccurredAt.Date);
        }
        else if(domainEvent is TaskStatusUpdated taskStatusUpdated)
        {
            IncrementStatusCount(taskStatusUpdated.ProjectId, taskStatusUpdated.NewStatusId, taskStatusUpdated.OccurredAt.Date);
            DecrementStatusCount(taskStatusUpdated.ProjectId, taskStatusUpdated.OldStatusId, taskStatusUpdated.OccurredAt.Date);
        }
    }

    private void IncrementStatusCount(Guid projectId, Guid statusId, DateTime date)
    {
        var currentDayProjection = Find(x => x.ProjectId == projectId && x.StatusId == statusId && x.Date.Date == date);

        if (currentDayProjection is null)
        {
            var previousDayProjection = GetPreviousDayProjection(projectId, statusId, date);
            var updatedCount = previousDayProjection is not null ? previousDayProjection.Count + 1 : 1;
            Add(new DailyTotalTaskStatus
            {
                ProjectId = projectId,
                StatusId = statusId,
                Date = date,
                Count = updatedCount
            });
        }
        else
        {
            ++currentDayProjection.Count;
        }
    }

    private void DecrementStatusCount(Guid projectId, Guid statusId, DateTime date)
    {
        var currentDayProjection = Find(x => x.ProjectId == projectId && x.StatusId == statusId && x.Date == date);

        if (currentDayProjection is null)
        {
            var previousDayProjection = GetPreviousDayProjection(projectId, statusId, date);
            var updatedCount = previousDayProjection is not null ? previousDayProjection.Count - 1 : 0;
            if(updatedCount <= 0)
            {
                return; // No need to add a projection with zero count
            }

            Add(new DailyTotalTaskStatus
            {
                ProjectId = projectId,
                StatusId = statusId,
                Date = date,
                Count = updatedCount
            });
        }
        else
        {
            --currentDayProjection.Count;
            if (currentDayProjection.Count <= 0)
            {
                Remove(currentDayProjection);
            }
        }
    }

    private DailyTotalTaskStatus? GetPreviousDayProjection(Guid projectId, Guid statusId, DateTime currentDate)
    {
        var previousDay = currentDate.AddDays(-1);
        return Find(x => x.ProjectId == projectId && x.StatusId == statusId && x.Date.Date == previousDay);
    }
}
