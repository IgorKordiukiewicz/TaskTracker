using Application.Features.Notifications;
using Domain.Notifications;
using Hangfire;

namespace Application.Common;

public interface IJobsService
{
    void AddExpireOrganizationsInvitationsJob();
    void EnqueueCreateNotification(NotificationData notification);
}

public class JobsService(IMediator mediator, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager) 
    : IJobsService
{

    public void AddExpireOrganizationsInvitationsJob()
    {
    }

    public void EnqueueCreateNotification(NotificationData notification)
    {
        backgroundJobClient.Enqueue(() => mediator.Send(new CreateNotificationCommand(notification), default));
    }
}
