
using Application.Features.Organizations;
using Application.Features.Notifications;
using Domain.Notifications;
using Hangfire;

namespace Application.Common;

public interface IJobsService
{
    void AddExpireOrganizationsInvitationsJob();
    void EnqueueCreateNotification(Notification notification);
}

public class JobsService(IMediator mediator, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager) 
    : IJobsService
{

    public void AddExpireOrganizationsInvitationsJob()
    {
        recurringJobManager.AddOrUpdate(
            "Expire organizations invitations",
            () => mediator.Send(new ExpireOrganizationsInvitationsCommand(), default),
            "0 * * * *");
    }

    public void EnqueueCreateNotification(Notification notification)
    {
        backgroundJobClient.Enqueue(() => mediator.Send(new CreateNotificationCommand(notification), default));
    }
}
