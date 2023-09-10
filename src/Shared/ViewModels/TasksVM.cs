namespace Shared.ViewModels;

public record TasksVM(IReadOnlyList<TaskVM> Tasks, IReadOnlyList<TaskStateVM> AllTaskStates);

public record TaskVM
{
    public required Guid Id { get; init; }
    public required int ShortId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required TaskStateVM State { get; init; }
    public required IReadOnlyList<TaskStateVM> AvailableStates { get; init; }
}

public record TaskStateVM(Guid Id, string Name);