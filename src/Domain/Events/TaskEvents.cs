namespace Domain.Events;

public record TaskCreated(Guid TaskId, Guid StatusId, Guid? AssigneeId, TaskPriority Priority, Guid ProjectId) : DomainEvent(ProjectId);

public record TaskAssigneeUpdated(Guid TaskId, Guid? OldAssigneeId, Guid? NewAssigneeId, Guid ProjectId) : DomainEvent(ProjectId);

public record TaskCommentAdded(Guid TaskId, Guid AuthorId, Guid ProjectId) : DomainEvent(ProjectId);

public record TaskEstimatedTimeUpdated(Guid TaskId, int? OldMinutes, int? NewMinutes, Guid ProjectId) : DomainEvent(ProjectId);

public record TaskPriorityUpdated(Guid TaskId, TaskPriority OldPriority, TaskPriority NewPriority, Guid ProjectId) : DomainEvent(ProjectId);

public record TaskStatusUpdated(Guid TaskId, Guid OldStatusId, Guid NewStatusId, Guid ProjectId) : DomainEvent(ProjectId);

public record TaskTimeLogged(Guid TaskId, int Minutes, DateOnly Day, Guid ProjectId) : DomainEvent(ProjectId);
