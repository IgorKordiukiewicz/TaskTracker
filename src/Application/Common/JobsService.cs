using Application.Features.Notifications;
using Application.Features.Projects;
using Domain.Notifications;
using Hangfire;

namespace Application.Common;

public interface IJobsService
{
    void AddExpireProjectsInvitationsJob();
    void EnqueueCreateNotification(NotificationData notification);
}

public class JobsService(IMediator mediator, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager) 
    : IJobsService
{

    public void AddExpireProjectsInvitationsJob()
    {
        recurringJobManager.AddOrUpdate(
            "Expire organizations invitations",
            () => mediator.Send(new ExpireProjectsInvitationsCommand(), default),
            "0 * * * *");
    }

    public void EnqueueCreateNotification(NotificationData notification)
    {
        backgroundJobClient.Enqueue(() => mediator.Send(new CreateNotificationCommand(notification), default));
    }
}
