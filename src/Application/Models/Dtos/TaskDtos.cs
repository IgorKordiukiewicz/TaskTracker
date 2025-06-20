﻿namespace Application.Models.Dtos;

public record CreateTaskDto
{
    public required string Title { get; init; }
    public string Description { get; init; } = string.Empty;
    public TaskPriority Priority { get; init; }
    public Guid? AssigneeMemberId { get; init; }
}

public record AddTaskCommentDto(string Content);
public record UpdateTaskStatusDto(Guid StatusId);
public record UpdateTaskAssigneeDto(Guid? MemberId);
public record UpdateTaskPriorityDto(TaskPriority Priority);
public record UpdateTaskDescriptionDto(string Description);
public record LogTaskTimeDto(int Minutes, DateTime Day);
public record UpdateTaskEstimatedTimeDto(int Minutes);
public record UpdateTaskTitleDto(string Title);

public record AddHierarchicalTaskRelationDto(Guid ParentId, Guid ChildId);
public record RemoveHierarchicalTaskRelationDto(Guid ParentId, Guid ChildId);

public record UpdateTaskBoardDto(Guid ProjectId, IReadOnlyCollection<UpdateTaskBoardColumnDto> Columns);
public record UpdateTaskBoardColumnDto(Guid StatusId, IReadOnlyCollection<Guid> TasksIds);