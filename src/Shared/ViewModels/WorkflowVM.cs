namespace Shared.ViewModels;

public record WorkflowVM(Guid Id, IReadOnlyList<WorkflowTaskStatusVM> Statuses, IReadOnlyList<WorkflowTaskStatusTransitionVM> Transitions);
public record WorkflowTaskStatusVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required bool Initial { get; init; }
    public required int DisplayOrder { get; init; }
    public required bool CanBeDeleted { get; init; }
}

public record WorkflowTaskStatusTransitionVM(Guid FromStatusId, Guid ToStatusId);
