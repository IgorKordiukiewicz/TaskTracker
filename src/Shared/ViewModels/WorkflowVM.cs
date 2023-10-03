namespace Shared.ViewModels;

public record WorkflowVM(Guid Id, IReadOnlyList<WorkflowTaskStatusVM> TaskStatuses);
public record WorkflowTaskStatusVM(Guid Id, string Name, bool IsInitial, int DisplayOrder, IReadOnlyList<Guid> PossibleNextStatuses);
