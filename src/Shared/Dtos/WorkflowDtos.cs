namespace Shared.Dtos;

public record AddWorkflowStatusDto(string Name);

public record AddWorkflowTransitionDto(Guid FromStatusId, Guid ToStatusId);