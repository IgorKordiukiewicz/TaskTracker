using Analytics.Infrastructure.Models;
using Analytics.ProjectionHandlers;
using Domain.Events;
using UnitTests.Analytics.Helpers;

namespace UnitTests.Analytics.ProjectionHandlers;

public class DailyTotalTaskStatusHandlerTests
{
    private readonly DateTime _now = new(2025, 05, 31, 12, 0, 0);
    private readonly Guid _projectId = Guid.NewGuid();

    [Fact]
    public async Task Creates_Projection_For_New_Day_Based_On_Last_Day_When_Task_Is_Created()
    {
        var status1Id = Guid.NewGuid();
        var previousDay = _now.AddDays(-1);

        var repository = new TestRepository
        {
            Projections =
            [
               CreateProjection(status1Id, previousDay, 1) // different day
            ]
        };
        var sut = new DailyTotalTaskStatusHandler(repository);
        await sut.InitializeState(_projectId);

        sut.ApplyEvent(new TaskCreated(Guid.NewGuid(), status1Id, null, default, _projectId, _now));

        using (new AssertionScope())
        {
            GetProjection(repository, status1Id, previousDay)!.Count.Should().Be(1); // previous day unchanged
            GetProjection(repository, status1Id, _now)!.Count.Should().Be(2); // current day created, based on previous day
        }
    }

    [Fact]
    public async Task Increments_Count_For_Existing_Day_When_Task_Is_Created()
    {
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();

        var repository = new TestRepository
        {
            Projections =
            [
               CreateProjection(status1Id, _now, 1), // current day projection
               CreateProjection(status2Id, _now, 5), // different status
            ]
        };
        var sut = new DailyTotalTaskStatusHandler(repository);
        await sut.InitializeState(_projectId);

        sut.ApplyEvent(new TaskCreated(Guid.NewGuid(), status1Id, null, default, _projectId, _now));

        GetProjection(repository, status1Id, _now)!.Count.Should().Be(2); // current day incremented
    }

    [Fact]
    public async Task Increments_And_Decrements_Counts_When_Task_Status_Is_Updated_On_Same_Day()
    {
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();

        var repository = new TestRepository
        {
            Projections =
            [
                CreateProjection(status1Id, _now, 5), // current day projection for status1
                CreateProjection(status2Id, _now, 5), // current day projection for status2
            ]
        };
        var sut = new DailyTotalTaskStatusHandler(repository);
        await sut.InitializeState(_projectId);

        sut.ApplyEvent(new TaskStatusUpdated(Guid.NewGuid(), status1Id, status2Id, _projectId, _now));

        using(new AssertionScope())
        {
            GetProjection(repository, status1Id, _now)!.Count.Should().Be(4); // current day status1 decremented
            GetProjection(repository, status2Id, _now)!.Count.Should().Be(6); // current day status2 incremented
        }
    }

    [Fact]
    public async Task Creates_Projections_For_Both_Statuses_When_Task_Status_Is_Updated_On_Different_Day()
    {
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();
        var previousDay = _now.AddDays(-1);

        var repository = new TestRepository
        {
            Projections =
            [
                CreateProjection(status1Id, previousDay, 5), // previous day projection for status1
                CreateProjection(status2Id, previousDay, 5), // previous day projection for status2
            ]
        };
        var sut = new DailyTotalTaskStatusHandler(repository);
        await sut.InitializeState(_projectId);

        sut.ApplyEvent(new TaskStatusUpdated(Guid.NewGuid(), status1Id, status2Id, _projectId, _now));

        using(new AssertionScope())
        {
            GetProjection(repository, status1Id, previousDay)!.Count.Should().Be(5); // previous day unchanged
            GetProjection(repository, status2Id, previousDay)!.Count.Should().Be(5); // previous day unchanged
            GetProjection(repository, status1Id, _now)!.Count.Should().Be(4); // current day created, based on previous day
            GetProjection(repository, status2Id, _now)!.Count.Should().Be(6); // current day created, based on previous day
        }
    }

    // If removed, on query the last projection will be matched, e.g.
    // day 1: 1, day 2: 1
    // after update -> day 1: 1, day 2: 0; query: 0
    // if removed -> day 1: 1, day 2: null, query: 1 (even though it should be 0)
    [Fact]
    public async Task Does_Not_Remove_Projection_When_Count_Is_Zero_After_Status_Update_On_Same_Day()
    {
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();

        var repository = new TestRepository
        {
            Projections =
            [
                CreateProjection(status1Id, _now, 1), // current day projection for status1
                CreateProjection(status2Id, _now, 5), // current day projection for status2
            ]
        };
        var sut = new DailyTotalTaskStatusHandler(repository);
        await sut.InitializeState(_projectId);

        sut.ApplyEvent(new TaskStatusUpdated(Guid.NewGuid(), status1Id, status2Id, _projectId, _now));

        using(new AssertionScope())
        {
            GetProjection(repository, status1Id, _now)!.Count.Should().Be(0); // current day decremented
            GetProjection(repository, status2Id, _now)!.Count.Should().Be(6); // current day incremented
        }
    }

    private DailyTotalTaskStatus? GetProjection(TestRepository repository, Guid statusId, DateTime now)
    {
        return repository.Projections
            .OfType<DailyTotalTaskStatus>()
            .FirstOrDefault(x => x.ProjectId == _projectId && x.StatusId == statusId && x.Date == now.Date);
    }

    private DailyTotalTaskStatus CreateProjection(Guid statusId, DateTime date, int count)
    {
        return new DailyTotalTaskStatus
        {
            ProjectId = _projectId,
            StatusId = statusId,
            Date = date.Date,
            Count = count
        };
    }
}
