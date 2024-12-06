namespace Domain.Notifications;

public static class NotificationFactory
{
    public static NotificationData TaskAssigned(Guid userId, Guid projectId, int taskShortId)
        => new(userId, $"Task #{taskShortId} has been assigned to you.", DateTime.Now, NotificationContext.Project, projectId, taskShortId);

    public static NotificationData TaskUnassigned(Guid userId, Guid projectId, int taskShortId)
        => new(userId, $"Task #{taskShortId} has been unassigned from you.", DateTime.Now, NotificationContext.Project, projectId, taskShortId);

    // TODO: Assigned task updated?

    public static NotificationData AddedToProject(Guid userId, Guid projectId)
        => new(userId, $"You have been added to the project.", DateTime.Now, NotificationContext.Project, projectId);

    public static NotificationData RemovedFromProject(Guid userId, Guid projectId)
        => new(userId, $"You have been removed from the project.", DateTime.Now, NotificationContext.Project, projectId);

    public static NotificationData RemovedFromOrganization(Guid userId, Guid organizationId)
        => new(userId, $"You have been removed from the organization.", DateTime.Now, NotificationContext.Organization, organizationId);
}