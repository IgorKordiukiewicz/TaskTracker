using Domain.Enums;

namespace Analytics.Infrastructure.Models;

public class DailyTotalTaskPriority : IDailyCountProjection
{
    public int Id { get; set; }
    public Guid ProjectId { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime Date { get; set; }
    public int Count { get; set; }
}
