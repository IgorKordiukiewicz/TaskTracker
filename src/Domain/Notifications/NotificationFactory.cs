namespace Domain.Notifications;

public static class NotificationFactory
{
    public static NotificationData TaskAssigned(Guid userId, DateTime now, Guid projectId, int taskShortId)
        => new(userId, $"Task #{taskShortId} has been assigned to you.", now, NotificationContext.Project, projectId, taskShortId);

    public static NotificationData TaskUnassigned(Guid userId, DateTime now, Guid projectId, int taskShortId)
        => new(userId, $"Task #{taskShortId} has been unassigned from you.", now, NotificationContext.Project, projectId, taskShortId);

    // TODO: Assigned task updated?

    public static NotificationData AddedToProject(Guid userId, DateTime now, Guid projectId)
        => new(userId, $"You have been added to the project.", now, NotificationContext.Project, projectId);

    public static NotificationData RemovedFromProject(Guid userId, DateTime now, Guid projectId)
        => new(userId, $"You have been removed from the project.", now, NotificationContext.Project, projectId);

    public static NotificationData RemovedFromOrganization(Guid userId, DateTime now, Guid organizationId)
        => new(userId, $"You have been removed from the organization.", now, NotificationContext.Organization, organizationId);

    public static NotificationData UserLeftOrganization(Guid receivingUserId, string userName, DateTime now, Guid organizationId)
        => new(receivingUserId, $"{userName} has left the organization.", now, NotificationContext.Organization, organizationId);
}