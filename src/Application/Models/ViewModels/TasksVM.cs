namespace Application.Models.ViewModels;

public record TasksVM(IList<TaskVM> Tasks, IReadOnlyList<TaskStatusDetailedVM> AllTaskStatuses, IReadOnlyList<TaskBoardColumnVM> BoardColumns);

public record TaskVM
{
    public required Guid Id { get; init; }
    public required int ShortId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required Guid? AssigneeId { get; init; }
    public required TaskPriority Priority { get; init; }
    public required TaskStatusVM Status { get; init; }
    public required IReadOnlyList<TaskStatusVM> PossibleNextStatuses { get; init; }
    public required int TotalTimeLogged { get; init; }
    public int? EstimatedTime { get; init; }
}

public record TaskBoardColumnVM(Guid StatusId, string StatusName, IReadOnlyList<Guid> PossibleNextStatuses, IReadOnlyList<Guid> TasksIds);

public record TaskStatusVM(Guid Id, string Name);
public record TaskStatusDetailedVM(Guid Id, string Name, int DisplayOrder) : TaskStatusVM(Id, Name);

public record TaskCommentsVM(IReadOnlyList<TaskCommentVM> Comments);
public record TaskCommentVM(string Content, Guid AuthorId, string AuthorName, DateTime CreatedAt);

public record TaskActivitiesVM(IReadOnlyList<TaskActivityVM> Activities);
public record TaskActivityVM(TaskProperty Property, string? OldValue, string? NewValue, DateTime OccurredAt);

public record TaskRelationshipsVM(Guid? ParentId, TaskHierarchyVM? ChildrenHierarchy);
public record TaskHierarchyVM(Guid TaskId, IReadOnlyList<TaskHierarchyVM> Children);