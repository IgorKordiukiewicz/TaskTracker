using Domain.Enums;

namespace Analytics.ViewModels;

public record TotalTaskStatusesVM(IReadOnlyDictionary<Guid, int> CountByStatusId);
public record TotalTaskStatusesByDayVM(IReadOnlyList<DateTime> Dates, IReadOnlyDictionary<Guid, IReadOnlyList<int>> CountsByStatusId);

public record TotalTaskPrioritiesVM(IReadOnlyDictionary<TaskPriority, int> CountByPriority);
public record TotalTaskPrioritiesByDayVM(IReadOnlyList<DateTime> Dates, IReadOnlyDictionary<TaskPriority, IReadOnlyList<int>> CountsByPriority);
