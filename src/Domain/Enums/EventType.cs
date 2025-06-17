namespace Domain.Enums;

public enum EventType
{
    TaskCreated,
    TaskAssigneeUpdated,
    TaskCommentAdded,
    TaskEstimatedTimeUpdated,
    TaskPriorityUpdated,
    TaskStatusUpdated,
    TaskTimeLogged,
    ProjectCreated,
    ProjectMemberLeft,
    ProjectMemberRemoved,
    ProjectInvitationCreated,
    ProjectInvitationAccepted,
    ProjectInvitationExpired,
    ProjectInvitationCanceled,
    ProjectInvitationDeclined,
    TaskAttachmentAdded,
}
