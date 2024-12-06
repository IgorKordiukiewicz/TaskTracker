namespace Domain.Notifications;

public static class NotificationFactory
{
    public static Notification TaskAssigned(Guid userId, Guid projectId, int taskShortId)
        => Notification.Create(userId, $"Task #{taskShortId} has been assigned to you.", DateTime.Now, NotificationContext.Project, projectId, taskShortId);

    public static Notification TaskUnassigned(Guid userId, Guid projectId, int taskShortId)
        => Notification.Create(userId, $"Task #{taskShortId} has been unassigned from you.", DateTime.Now, NotificationContext.Project, projectId, taskShortId);

    // TODO: Assigned task updated?

    public static Notification AddedToProject(Guid userId, Guid projectId)
        => Notification.Create(userId, $"You have been added to the project.", DateTime.Now, NotificationContext.Project, projectId);

    public static Notification RemovedFromProject(Guid userId, Guid projectId)
        => Notification.Create(userId, $"You have been removed from the project.", DateTime.Now, NotificationContext.Project, projectId);

    public static Notification RemovedFromOrganization(Guid userId, Guid organizationId)
        => Notification.Create(userId, $"You have been removed from the organization.", DateTime.Now, NotificationContext.Organization, organizationId);
}
