using Analytics.Infrastructure;
using Analytics.Infrastructure.Models;
using Domain.Common;
using Domain.Events;

namespace Analytics.ProjectionHandlers;

public class DailyTotalTaskStatusHandler(AnalyticsDbContext dbContext)
    : IProjectionHandler
{
    public async Task ApplyEvent(DomainEvent domainEvent)
    {
        if(domainEvent is TaskCreated taskCreated)
        {
            await IncrementStatusCount(taskCreated.ProjectId, taskCreated.StatusId, taskCreated.OccurredAt.Date);
        }
        else if(domainEvent is TaskStatusUpdated taskStatusUpdated)
        {
            await IncrementStatusCount(taskStatusUpdated.ProjectId, taskStatusUpdated.NewStatusId, taskStatusUpdated.OccurredAt.Date);
            await DecrementStatusCount(taskStatusUpdated.ProjectId, taskStatusUpdated.OldStatusId, taskStatusUpdated.OccurredAt.Date);
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task IncrementStatusCount(Guid projectId, Guid statusId, DateTime date)
    {
        var projection = await GetProjection(projectId, statusId, date);

        if (projection is null)
        {
            dbContext.DailyTotalTaskStatuses.Add(new DailyTotalTaskStatus
            {
                ProjectId = projectId,
                StatusId = statusId,
                Date = date,
                Count = 1
            });
        }
        else
        {
            ++projection.Count;
            dbContext.DailyTotalTaskStatuses.Update(projection);
        }
    }

    private async Task DecrementStatusCount(Guid projectId, Guid statusId, DateTime date)
    {
        var projection = await GetProjection(projectId, statusId, date);

        if (projection is null)
        {
            return; // TODO: throw exception?
        }

        --projection.Count;
        if (projection.Count <= 0)
        {
            dbContext.DailyTotalTaskStatuses.Remove(projection);
        }
        else
        {
            dbContext.DailyTotalTaskStatuses.Update(projection);
        }
    }

    private async Task<DailyTotalTaskStatus?> GetProjection(Guid projectId, Guid statusId, DateTime date)
        => await dbContext.DailyTotalTaskStatuses.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.StatusId == statusId && x.Date == date);
}
