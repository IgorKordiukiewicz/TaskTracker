namespace Domain.Notifications;

public static class NotificationFactory
{
    public static NotificationData TaskAssigned(Guid userId, DateTime now, Guid projectId, int taskShortId)
        => new(userId, $"Task #{taskShortId} has been assigned to you.", now, projectId, taskShortId);

    public static NotificationData TaskUnassigned(Guid userId, DateTime now, Guid projectId, int taskShortId)
        => new(userId, $"Task #{taskShortId} has been unassigned from you.", now, projectId, taskShortId);

    // TODO: Assigned task updated?

    public static NotificationData AddedToProject(Guid userId, DateTime now, Guid projectId)
        => new(userId, $"You have been added to the project.", now, projectId);

    public static NotificationData RemovedFromProject(Guid userId, DateTime now, Guid projectId)
        => new(userId, $"You have been removed from the project.", now, projectId);
}