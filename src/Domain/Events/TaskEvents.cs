namespace Domain.Events;

public record TaskCreated(Guid TaskId, Guid StatusId, Guid? AssigneeId, TaskPriority Priority, Guid ProjectId, DateTime OccurredAt) 
    : DomainEvent(ProjectId, EventType.TaskCreated, OccurredAt);

public record TaskAssigneeUpdated(Guid TaskId, Guid? OldAssigneeId, Guid? NewAssigneeId, Guid ProjectId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.TaskAssigneeUpdated, OccurredAt);

public record TaskCommentAdded(Guid TaskId, Guid AuthorId, Guid ProjectId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.TaskCommentAdded, OccurredAt);

public record TaskEstimatedTimeUpdated(Guid TaskId, int? OldMinutes, int? NewMinutes, Guid ProjectId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.TaskEstimatedTimeUpdated, OccurredAt);

public record TaskPriorityUpdated(Guid TaskId, TaskPriority OldPriority, TaskPriority NewPriority, Guid ProjectId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.TaskPriorityUpdated, OccurredAt);

public record TaskStatusUpdated(Guid TaskId, Guid OldStatusId, Guid NewStatusId, Guid ProjectId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.TaskStatusUpdated, OccurredAt);

public record TaskTimeLogged(Guid TaskId, int Minutes, DateOnly Day, Guid ProjectId, DateTime OccurredAt)
   : DomainEvent(ProjectId, EventType.TaskTimeLogged, OccurredAt);

public record TaskAttachmentAdded(Guid TaskId, string FileName, Guid ProjectId, DateTime OccurredAt)
    : DomainEvent(ProjectId, EventType.TaskAttachmentAdded, OccurredAt);