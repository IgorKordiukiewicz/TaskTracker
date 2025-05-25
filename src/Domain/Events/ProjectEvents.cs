namespace Domain.Events;

public record ProjectCreated(Guid ProjectId, Guid OwnerId) : DomainEvent(ProjectId);
public record ProjectMemberLeft(Guid ProjectId, Guid UserId) : DomainEvent(ProjectId);

public record ProjectMemberRemoved(Guid ProjectId, Guid UserId) : DomainEvent(ProjectId);

public record ProjectInvitationCreated(Guid ProjectId, Guid UserId) : DomainEvent(ProjectId);

public record ProjectInvitationAccepted(Guid ProjectId, Guid UserId) : DomainEvent(ProjectId);

public record ProjectInvitationExpired(Guid ProjectId, Guid UserId) : DomainEvent(ProjectId);

public record ProjectInvitationCanceled(Guid ProjectId, Guid UserId) : DomainEvent(ProjectId);

public record ProjectInvitationDeclined(Guid ProjectId, Guid UserId) : DomainEvent(ProjectId);