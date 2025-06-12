using Analytics.Infrastructure.Models;
using Analytics.Services;
using Domain.Common;
using Domain.Events;

namespace Analytics.ProjectionHandlers;

public class DailyTotalTaskAssigneeHandler(IRepository repository)
    : DailyTotalTaskPropertyHandler<DailyTotalTaskAssignee, Guid>(repository, (x, property) => x.AssigneeId == property)
{
    public override void ApplyEvent(DomainEvent domainEvent)
    {
        if(domainEvent is TaskCreated taskCreated)
        {
            UpdateStatusCount(taskCreated.ProjectId, taskCreated.AssigneeId ?? Guid.Empty, taskCreated.OccurredAt.Date);
        }
        else if(domainEvent is TaskAssigneeUpdated taskAssigneeUpdated)
        {
            UpdateStatusCount(taskAssigneeUpdated.ProjectId, taskAssigneeUpdated.NewAssigneeId ?? Guid.Empty, taskAssigneeUpdated.OccurredAt.Date);
            UpdateStatusCount(taskAssigneeUpdated.ProjectId, taskAssigneeUpdated.OldAssigneeId ?? Guid.Empty, taskAssigneeUpdated.OccurredAt.Date, false);
        }
    }

    protected override DailyTotalTaskAssignee CreateProjection(Guid projectId, DateTime date, Guid property, int count)
    {
        return new()
        {
            ProjectId = projectId,
            Date = date,
            AssigneeId = property,
            Count = count
        };
    }
}
