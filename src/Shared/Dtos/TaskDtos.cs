﻿using Shared.Enums;

namespace Shared.Dtos;

public record CreateTaskDto
{
    public required string Title { get; init; }
    public string Description { get; init; } = string.Empty;
    public TaskPriority Priority { get; init; }
    public Guid? AssigneeMemberId { get; init; }
}

public record AddTaskCommentDto(string Content);
public record UpdateTaskAssigneeDto(Guid? MemberId);
public record UpdateTaskPriorityDto(TaskPriority Priority);
public record UpdateTaskDescriptionDto(string Description);