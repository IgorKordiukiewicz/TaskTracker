
using Domain.Notifications;

namespace Application.Features.Notifications;

public record CreateNotificationCommand(NotificationData Data) : IRequest;

internal class CreateNotificationHandler(IRepository<Notification> notificationRepository)
    : IRequestHandler<CreateNotificationCommand>
{
    public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        await notificationRepository.Add(Notification.FromData(request.Data), cancellationToken);
    }
}
