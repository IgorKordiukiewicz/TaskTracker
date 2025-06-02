namespace Domain.Events;

public record ProjectCreated(Guid ProjectId, Guid OwnerId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.ProjectCreated, Now);
public record ProjectMemberLeft(Guid ProjectId, Guid UserId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.ProjectMemberLeft, Now);

public record ProjectMemberRemoved(Guid ProjectId, Guid UserId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.ProjectMemberRemoved, Now);

public record ProjectInvitationCreated(Guid ProjectId, Guid UserId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.ProjectInvitationCreated, Now);

public record ProjectInvitationAccepted(Guid ProjectId, Guid UserId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.ProjectInvitationAccepted, Now);

public record ProjectInvitationExpired(Guid ProjectId, Guid UserId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.ProjectInvitationExpired, Now);

public record ProjectInvitationCanceled(Guid ProjectId, Guid UserId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.ProjectInvitationCanceled, Now);

public record ProjectInvitationDeclined(Guid ProjectId, Guid UserId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.ProjectInvitationDeclined, Now);
