namespace Domain.Events;

public record ProjectCreated(Guid ProjectId, Guid OwnerId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.ProjectCreated, OccurredAt);
public record ProjectMemberLeft(Guid ProjectId, Guid UserId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.ProjectMemberLeft, OccurredAt);

public record ProjectMemberRemoved(Guid ProjectId, Guid UserId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.ProjectMemberRemoved, OccurredAt);

public record ProjectInvitationCreated(Guid ProjectId, Guid UserId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.ProjectInvitationCreated, OccurredAt);

public record ProjectInvitationAccepted(Guid ProjectId, Guid UserId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.ProjectInvitationAccepted, OccurredAt);

public record ProjectInvitationExpired(Guid ProjectId, Guid UserId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.ProjectInvitationExpired, OccurredAt);

public record ProjectInvitationCanceled(Guid ProjectId, Guid UserId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.ProjectInvitationCanceled, OccurredAt);

public record ProjectInvitationDeclined(Guid ProjectId, Guid UserId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.ProjectInvitationDeclined, OccurredAt);
