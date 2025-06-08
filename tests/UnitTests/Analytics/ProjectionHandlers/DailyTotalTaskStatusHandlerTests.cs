using Analytics.Infrastructure.Models;
using Analytics.ProjectionHandlers;
using Domain.Events;
using UnitTests.Analytics.Helpers;

namespace UnitTests.Analytics.ProjectionHandlers;

public class DailyTotalTaskStatusHandlerTests 
    : DailyTotalTaskPropertyHandlerTestsBase<DailyTotalTaskStatus, Guid, DailyTotalTaskStatusHandler>
{
    public DailyTotalTaskStatusHandlerTests() 
        : base(
            (x, property) => x.StatusId == property, 
            (x) => new DailyTotalTaskStatusHandler(x),
            Guid.NewGuid(), Guid.NewGuid())
    {
    }

    protected override DailyTotalTaskStatus CreateProjection(Guid statusId, DateTime date, int count)
    {
        return new()
        {
            ProjectId = _projectId,
            StatusId = statusId,
            Date = date.Date,
            Count = count
        };
    }

    [Fact]
    public async Task Creates_Projection_For_New_Day_Based_On_Last_Day_When_Task_Is_Created()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _twoDaysAgo, 1),
            CreateProjection(_property1, _previousDay, 2)
        );

        sut.ApplyEvent(new TaskCreated(Guid.NewGuid(), _property1, null, default, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _twoDaysAgo).ShouldHaveCount(1); // two days ago unchanged
            GetProjection(repository, _property1, _previousDay).ShouldHaveCount(2); // previous day unchanged
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(3); // current day created, based on previous day
        }
    }

    [Fact]
    public async Task Increments_Count_For_Existing_Day_When_Task_Is_Created()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _currentDay, 1), // current day projection
            CreateProjection(_property2, _currentDay, 5) // different status
        );

        sut.ApplyEvent(new TaskCreated(Guid.NewGuid(), _property1, null, default, _projectId, _currentDay));

        GetProjection(repository, _property1, _currentDay).ShouldHaveCount(2); // current day incremented
    }

    [Fact]
    public async Task Increments_And_Decrements_Counts_When_Task_Status_Is_Updated_On_Same_Day()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _currentDay, 5), // current day projection for status1
            CreateProjection(_property2, _currentDay, 5) // current day projection for status2
        );

        sut.ApplyEvent(new TaskStatusUpdated(Guid.NewGuid(), _property1, _property2, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(4); // current day status1 decremented
            GetProjection(repository, _property2, _currentDay).ShouldHaveCount(6); // current day status2 incremented
        }
    }

    [Fact]
    public async Task Creates_Projections_For_Both_Statuses_When_Task_Status_Is_Updated_On_Different_Day()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _previousDay, 5), // previous day projection for status1
            CreateProjection(_property2, _previousDay, 5) // previous day projection for status2
        );

        sut.ApplyEvent(new TaskStatusUpdated(Guid.NewGuid(), _property1, _property2, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _previousDay).ShouldHaveCount(5); // previous day unchanged
            GetProjection(repository, _property2, _previousDay).ShouldHaveCount(5); // previous day unchanged
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(4); // current day created, based on previous day
            GetProjection(repository, _property2, _currentDay).ShouldHaveCount(6); // current day created, based on previous day
        }
    }

    // If removed, when querying, the last projection will be matched, e.g.
    // day 1: 1, day 2: 1
    // after update -> day 1: 1, day 2: 0; query: 0
    // if removed -> day 1: 1, day 2: null, query: 1 (even though it should be 0)
    [Fact]
    public async Task Does_Not_Remove_Projection_When_Count_Is_Zero_After_Status_Update_On_Same_Day()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _currentDay, 1), // previous day projection for status1
            CreateProjection(_property2, _currentDay, 5) // previous day projection for status2
        );

        sut.ApplyEvent(new TaskStatusUpdated(Guid.NewGuid(), _property1, _property2, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(0); // current day decremented
            GetProjection(repository, _property2, _currentDay).ShouldHaveCount(6); // current day incremented
        }
    }
}
