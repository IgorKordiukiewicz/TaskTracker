using Analytics.Infrastructure.Models;
using Analytics.Services;

namespace UnitTests.Analytics.Services;

public class ModelBuilderTests
{
    [Fact]
    public void Builds_Total_Task_Statuses_By_Day()
    {
        var status1 = Guid.NewGuid();
        var status2 = Guid.NewGuid();

        var dailyTotalTaskStatuses = new List<DailyTotalTaskStatus>
        {
            CreateProjection(status1, new DateTime(2025, 6, 1), 1),
            CreateProjection(status1, new DateTime(2025, 6, 2), 3),
            CreateProjection(status1, new DateTime(2025, 6, 4), 7),
            CreateProjection(status2, new DateTime(2025, 6, 2), 4)
        };

        static DailyTotalTaskStatus CreateProjection(Guid statusId, DateTime date, int Count)
            => new()
            {
                Id = 0,
                ProjectId = Guid.NewGuid(),
                StatusId = statusId,
                Date = date,
                Count = Count
            };

        var result = ModelBuilder.BuildTotalTaskStatusesByDay(dailyTotalTaskStatuses);

        using(new AssertionScope())
        {
            result.Dates.Should().BeEquivalentTo(
            [ 
                new DateTime(2025, 6, 1),
                new DateTime(2025, 6, 2),
                new DateTime(2025, 6, 3),
                new DateTime(2025, 6, 4)
            ]);

            var status1Counts = result.CountsByStatusId[status1];
            status1Counts.Should().BeEquivalentTo([1, 3, 3, 7]);

            var status2Counts = result.CountsByStatusId[status2];
            status2Counts.Should().BeEquivalentTo([0, 4, 4, 4]);
        }
    }
}
