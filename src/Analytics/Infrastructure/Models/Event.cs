using Domain.Enums;

namespace Analytics.Infrastructure.Models;

public class Event
{
    public int Id { get; init; }

    public required Guid ProjectId { get; init; }

    public EventType Type { get; init; }

    public required string Details { get; init; }

    public required DateTime OccurredAt { get; init; }
}