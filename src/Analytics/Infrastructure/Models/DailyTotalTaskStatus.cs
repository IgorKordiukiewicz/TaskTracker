namespace Analytics.Infrastructure.Models;

public class DailyTotalTaskStatus : IProjection
{
    public Guid ProjectId { get; set; }
    public Guid StatusId { get; set; }
    public DateTime Date { get; set; }
    public int Count { get; set; }
}
