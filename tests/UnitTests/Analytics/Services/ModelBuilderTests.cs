using Analytics.Services;
using UnitTests.Analytics.Helpers;

namespace UnitTests.Analytics.Services;

public class ModelBuilderTests
{
    [Fact]
    public void Builds_Total_Task_Properties_By_Day()
    {
        var dailyTotalTaskProperties = new List<TestProjection>
        {
            CreateProjection(1, new DateTime(2025, 6, 1), 1),
            CreateProjection(1, new DateTime(2025, 6, 2), 3),
            CreateProjection(1, new DateTime(2025, 6, 4), 7),
            CreateProjection(2, new DateTime(2025, 6, 2), 4)
        };

        static TestProjection CreateProjection(int property, DateTime date, int Count)
            => new()
            {
                Id = 0,
                ProjectId = Guid.NewGuid(),
                Property = property,
                Date = date,
                Count = Count
            };

        var result = ModelBuilder.BuildTotalTaskPropertiesByDay(dailyTotalTaskProperties,
            x => x.Property,
            (x, property) => x.Property == property);

        using(new AssertionScope())
        {
            result.Dates.Should().BeEquivalentTo(
            [ 
                new DateTime(2025, 6, 1),
                new DateTime(2025, 6, 2),
                new DateTime(2025, 6, 3),
                new DateTime(2025, 6, 4)
            ]);

            var status1Counts = result.DailyCountByProperty[1];
            status1Counts.Should().BeEquivalentTo([1, 3, 3, 7]);

            var status2Counts = result.DailyCountByProperty[2];
            status2Counts.Should().BeEquivalentTo([0, 4, 4, 4]);
        }
    }
}
