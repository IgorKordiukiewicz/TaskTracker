namespace Analytics.Infrastructure.Models;

public class DailyTotalTaskStatus : IDailyCountProjection
{
    public int Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid StatusId { get; set; }
    public DateTime Date { get; set; }
    public int Count { get; set; }
}
