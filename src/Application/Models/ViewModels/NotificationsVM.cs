using Domain.Notifications;

namespace Application.Models.ViewModels;

public record NotificationsVM(IReadOnlyList<NotificationVM> Notifications);
public record NotificationVM(Guid Id, string Message, DateTime OccurredAt, Guid ContextEntityId, string ContextEntityName, int? TaskShortId);
