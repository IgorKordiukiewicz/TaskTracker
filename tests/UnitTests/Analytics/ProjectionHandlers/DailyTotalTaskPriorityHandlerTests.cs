using Analytics.Infrastructure.Models;
using Analytics.ProjectionHandlers;
using Domain.Events;
using UnitTests.Analytics.Helpers;

namespace UnitTests.Analytics.ProjectionHandlers;

public class DailyTotalTaskPriorityHandlerTests
    : DailyTotalTaskPropertyHandlerTestsBase<DailyTotalTaskPriority, TaskPriority, DailyTotalTaskPriorityHandler>
{
    public DailyTotalTaskPriorityHandlerTests()
        : base(
            (x, property) => x.Priority == property, 
            (x) => new DailyTotalTaskPriorityHandler(x),
            TaskPriority.Normal, TaskPriority.High)
    {
    }

    protected override DailyTotalTaskPriority CreateProjection(TaskPriority priority, DateTime date, int count)
    {
        return new DailyTotalTaskPriority
        {
            ProjectId = _projectId,
            Priority = priority,
            Date = date.Date,
            Count = count
        };
    }

    [Fact]
    public async Task Creates_Projection_For_New_Day_Based_On_Last_Day_When_Task_Is_Created()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _previousDay, 1)
        );

        sut.ApplyEvent(new TaskCreated(Guid.NewGuid(), Guid.NewGuid(), null, _property1, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _previousDay).ShouldHaveCount(1); // previous day unchanged
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(2); // current day created, based on previous day
        }
    }

    [Fact]
    public async Task Increments_Count_For_Existing_Day_When_Task_Is_Created()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _currentDay, 1), // current day projection
            CreateProjection(_property2, _currentDay, 5) // different priority
        );

        sut.ApplyEvent(new TaskCreated(Guid.NewGuid(), Guid.NewGuid(), null, _property1, _projectId, _currentDay));

        GetProjection(repository, _property1, _currentDay).ShouldHaveCount(2); // current day incremented
    }

    [Fact]
    public async Task Increments_And_Decrements_Counts_When_Task_Priority_Is_Updated_On_Same_Day()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _currentDay, 5), // current day projection for priority1
            CreateProjection(_property2, _currentDay, 5) // current day projection for priority2
        );

        sut.ApplyEvent(new TaskPriorityUpdated(Guid.NewGuid(), _property1, _property2, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(4); // current day priority1 decremented
            GetProjection(repository, _property2, _currentDay).ShouldHaveCount(6); // current day priority2 incremented
        }
    }

    [Fact]
    public async Task Creates_Projections_For_Both_Priorities_When_Task_Priority_Is_Updated_On_Different_Day()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _previousDay, 5), // previous day projection for priority1
            CreateProjection(_property2, _previousDay, 5) // previous day projection for priority2
        );

        sut.ApplyEvent(new TaskPriorityUpdated(Guid.NewGuid(), _property1, _property2, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _previousDay).ShouldHaveCount(5); // previous day unchanged
            GetProjection(repository, _property2, _previousDay).ShouldHaveCount(5); // previous day unchanged
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(4); // current day created, based on previous day
            GetProjection(repository, _property2, _currentDay).ShouldHaveCount(6); // current day created, based on previous day
        }
    }

    [Fact]
    public async Task Does_Not_Remove_Projection_When_Count_Is_Zero_After_Priority_Update_On_Same_Day()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _currentDay, 1), // previous day projection for priority1
            CreateProjection(_property2, _currentDay, 5) // previous day projection for priority2
        );

        sut.ApplyEvent(new TaskPriorityUpdated(Guid.NewGuid(), _property1, _property2, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(0); // current day decremented
            GetProjection(repository, _property2, _currentDay).ShouldHaveCount(6); // current day incremented
        }
    }
}
