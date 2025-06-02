namespace Domain.Events;

public record TaskCreated(Guid TaskId, Guid StatusId, Guid? AssigneeId, TaskPriority Priority, Guid ProjectId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.TaskCreated, Now);

public record TaskAssigneeUpdated(Guid TaskId, Guid? OldAssigneeId, Guid? NewAssigneeId, Guid ProjectId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.TaskAssigneeUpdated, Now);

public record TaskCommentAdded(Guid TaskId, Guid AuthorId, Guid ProjectId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.TaskCommentAdded, Now);

public record TaskEstimatedTimeUpdated(Guid TaskId, int? OldMinutes, int? NewMinutes, Guid ProjectId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.TaskEstimatedTimeUpdated, Now);

public record TaskPriorityUpdated(Guid TaskId, TaskPriority OldPriority, TaskPriority NewPriority, Guid ProjectId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.TaskPriorityUpdated, Now);

public record TaskStatusUpdated(Guid TaskId, Guid OldStatusId, Guid NewStatusId, Guid ProjectId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.TaskStatusUpdated, Now);

public record TaskTimeLogged(Guid TaskId, int Minutes, DateOnly Day, Guid ProjectId, DateTime? Now = null) 
    : DomainEvent(ProjectId, EventType.TaskTimeLogged, Now);
