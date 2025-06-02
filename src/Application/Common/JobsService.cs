using Analytics.Services;
using Application.Features.Notifications;
using Application.Features.Projects;
using Domain.Notifications;
using Hangfire;

namespace Application.Common;

public interface IJobsService
{
    void AddCRONJobs();
    void EnqueueCreateNotification(NotificationData notification);
}

public class JobsService(IMediator mediator, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager, IProjectionRebuilder projectionRebuilder) 
    : IJobsService
{

    public void AddCRONJobs()
    {
        recurringJobManager.AddOrUpdate(
            "Expire projects invitations",
            () => mediator.Send(new ExpireProjectsInvitationsCommand(), default),
            "0 * * * *");

        recurringJobManager.AddOrUpdate(
            "Rebuild projections",
            () => projectionRebuilder.RebuildProjections(),
            Cron.Never);
    }

    public void EnqueueCreateNotification(NotificationData notification)
    {
        backgroundJobClient.Enqueue(() => mediator.Send(new CreateNotificationCommand(notification), default));
    }
}
