namespace Shared.ViewModels;

public record WorkflowVM(Guid Id, IReadOnlyList<WorkflowTaskStatusVM> TaskStatuses);
public record WorkflowTaskStatusVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsInitial { get; init; }
    public required int DisplayOrder { get; init; }
    public required IReadOnlyList<Guid> PossibleNextStatuses { get; init; }
    public required bool CanBeDeleted { get; init; }
}
