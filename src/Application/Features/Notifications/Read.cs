using Domain.Notifications;

namespace Application.Features.Notifications;

public record ReadNotificationCommand(Guid NotificationId, Guid UserId) : IRequest<Result>;

internal class ReadNotificationCommandValidator : AbstractValidator<ReadNotificationCommand>
{
    public ReadNotificationCommandValidator()
    {
        RuleFor(x => x.NotificationId).NotEmpty();
    }
}

internal class ReadNotificationHandler(IRepository<Notification> notificationRepository)
    : IRequestHandler<ReadNotificationCommand, Result>
{
    public async Task<Result> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetById(request.NotificationId, cancellationToken);
        if(notification is null)
        {
            return Result.Fail(new NotFoundError<Notification>(request.NotificationId));
        }

        if(notification.UserId != request.UserId)
        {
            return Result.Fail(new ApplicationError("This notification does not belong to the current user."));
        }

        notification.MarkAsRead();

        await notificationRepository.Update(notification, cancellationToken);
        return Result.Ok();
    }
}
