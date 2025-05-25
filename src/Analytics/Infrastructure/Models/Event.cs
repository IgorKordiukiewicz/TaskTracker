using System.ComponentModel.DataAnnotations;

namespace Analytics.Infrastructure.Models;

public class Event
{
    public int Id { get; init; }

    public required Guid ProjectId { get; init; }

    // TODO: Event type enum?

    public required string Details { get; init; }

    public required DateTime OccurredAt { get; init; }
}