using Domain.Enums;

namespace Analytics.ViewModels;

public record TaskAnalyticsVM
{
    public required IReadOnlyList<DateTime> Dates { get; init; }

    public required IReadOnlyDictionary<Guid, int> CountByStatusId { get; init; }
    public required IReadOnlyDictionary<Guid, IReadOnlyList<int>> DailyCountByStatusId { get; init; }

    public required IReadOnlyDictionary<TaskPriority, int> CountByPriority { get; init; }
    public required IReadOnlyDictionary<TaskPriority, IReadOnlyList<int>> DailyCountByPriority { get; init; }

    public required IReadOnlyDictionary<Guid, int> CountByAssigneeId { get; init; }
    public required IReadOnlyDictionary<Guid, IReadOnlyList<int>> DailyCountByAssigneeId { get; init; }
}