namespace Analytics.ViewModels;

public record TotalTaskStatusesVM(IReadOnlyDictionary<Guid, int> CountByStatusId);

public record TotalTaskStatusesByDayVM(IReadOnlyList<DateTime> Dates, IReadOnlyDictionary<Guid, IReadOnlyList<int>> CountsByStatusId);