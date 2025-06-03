using Analytics.Infrastructure.Models;

namespace UnitTests.Analytics.Helpers;

public class TestProjection : IDailyCountProjection
{
    public int Id { get; set; }
    public Guid ProjectId { get; set; }
    public int Property { get; set; }
    public DateTime Date { get; set; }
    public int Count { get; set; }
}
