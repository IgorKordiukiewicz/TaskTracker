namespace Application.Models.Dtos;

public record AddWorkflowStatusDto(string Name);

public record AddWorkflowTransitionDto(Guid FromStatusId, Guid ToStatusId);

public record DeleteWorkflowStatusDto(Guid StatusId);
public record DeleteWorkflowTransitionDto(Guid FromStatusId, Guid ToStatusId);

public record ChangeInitialWorkflowStatusDto(Guid StatusId);