using Analytics.Infrastructure.Models;
using Analytics.ProjectionHandlers;
using Domain.Events;
using UnitTests.Analytics.Helpers;

namespace UnitTests.Analytics.ProjectionHandlers;

public class DailyTotalTaskAssigneeHandlerTests
    : DailyTotalTaskPropertyHandlerTestsBase<DailyTotalTaskAssignee, Guid, DailyTotalTaskAssigneeHandler>
{
    public DailyTotalTaskAssigneeHandlerTests()
        : base(
            (x, property) => x.AssigneeId == property,
            (x) => new DailyTotalTaskAssigneeHandler(x),
            Guid.NewGuid(), Guid.Empty)
    {
    }

    protected override DailyTotalTaskAssignee CreateProjection(Guid assigneeId, DateTime date, int count)
    {
        return new()
        {
            ProjectId = _projectId,
            AssigneeId = assigneeId,
            Date = date.Date,
            Count = count
        };
    }

    [Fact]
    public async Task Creates_Projection_For_New_Day_Based_On_Last_Day_When_Task_Is_Created()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _threeDaysAgo, 1),
            CreateProjection(_property1, _twoDaysAgo, 2)
        );

        sut.ApplyEvent(new TaskCreated(Guid.NewGuid(), Guid.NewGuid(), _property1, default, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _threeDaysAgo).ShouldHaveCount(1); // three days ago unchanged
            GetProjection(repository, _property1, _twoDaysAgo).ShouldHaveCount(2); // two days ago unchanged
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(3); // current day created, based on last day
        }
    }

    [Fact]
    public async Task Increments_Count_For_Existing_Day_When_Task_Is_Created()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _currentDay, 1), // current day projection
            CreateProjection(_property2, _currentDay, 5) // different assignee
        );

        sut.ApplyEvent(new TaskCreated(Guid.NewGuid(), Guid.NewGuid(), _property1, default, _projectId, _currentDay));

        GetProjection(repository, _property1, _currentDay).ShouldHaveCount(2); // current day incremented
    }

    [Fact]
    public async Task Increments_And_Decrements_Counts_When_Task_Assignee_Is_Updated_On_Same_Day()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _currentDay, 5), // current day projection for assignee1
            CreateProjection(_property2, _currentDay, 5) // current day projection for assignee2
        );

        sut.ApplyEvent(new TaskAssigneeUpdated(Guid.NewGuid(), _property1, _property2, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(4); // current day status1 decremented
            GetProjection(repository, _property2, _currentDay).ShouldHaveCount(6); // current day status2 incremented
        }
    }

    [Fact]
    public async Task Creates_Projections_For_Both_Assignees_When_Task_Assignee_Is_Updated_On_Different_Day()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _previousDay, 5), // previous day projection for assignee1
            CreateProjection(_property2, _previousDay, 5) // previous day projection for assignee2
        );

        sut.ApplyEvent(new TaskAssigneeUpdated(Guid.NewGuid(), _property1, _property2, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _previousDay).ShouldHaveCount(5); // previous day unchanged
            GetProjection(repository, _property2, _previousDay).ShouldHaveCount(5); // previous day unchanged
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(4); // current day created, based on previous day
            GetProjection(repository, _property2, _currentDay).ShouldHaveCount(6); // current day created, based on previous day
        }
    }

    [Fact]
    public async Task Does_Not_Remove_Projection_When_Count_Is_Zero_After_Assignee_Update_On_Same_Day()
    {
        var (sut, repository) = await Arrange(
            CreateProjection(_property1, _currentDay, 1), // previous day projection for assignee1
            CreateProjection(_property2, _currentDay, 5) // previous day projection for assignee2
        );

        sut.ApplyEvent(new TaskAssigneeUpdated(Guid.NewGuid(), _property1, _property2, _projectId, _currentDay));

        using (new AssertionScope())
        {
            GetProjection(repository, _property1, _currentDay).ShouldHaveCount(0); // current day decremented
            GetProjection(repository, _property2, _currentDay).ShouldHaveCount(6); // current day incremented
        }
    }
}
