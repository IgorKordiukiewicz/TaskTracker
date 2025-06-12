
using Domain.Enums;

namespace Analytics.Infrastructure.Models;

public class DailyTotalTaskAssignee : IDailyCountProjection
{
    public int Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid AssigneeId { get; set; } // No assignee is represented by Guid.Empty
    public DateTime Date { get; set; }
    public int Count { get; set; }
}
