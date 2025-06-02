namespace Analytics.ViewModels;

public record TotalTaskStatusesVM(IReadOnlyDictionary<Guid, int> CountByStatusId);
