using Domain.Tasks;
using Task = Domain.Tasks.Task;

namespace UnitTests.Domain;

public class TaskTests
{
    [Fact]
    public void Create_ShouldCreateTask_WithGivenParameters()
    {
        var shortId = 1;
        var projectId = Guid.NewGuid();
        var title = "Title";
        var description = "Description";
        var statusId = Guid.NewGuid();

        var result = Task.Create(shortId, projectId, title, description, statusId);

        using(new AssertionScope())
        {
            result.Id.Should().NotBeEmpty();
            result.ShortId.Should().Be(shortId);
            result.ProjectId.Should().Be(projectId);
            result.Title.Should().Be(title);
            result.Description.Should().Be(description);
            result.StatusId.Should().Be(statusId);
        }
    }

    [Fact]
    public void UpdateStatus_ShouldFail_WhenNewStatusIdIsNotValid()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var initialStatus = workflow.Statuses.First(x => x.IsInitial);
        var task = Task.Create(1, Guid.NewGuid(), "title", "desc", initialStatus.Id);

        var result = task.UpdateStatus(Guid.NewGuid(), workflow);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateStatus_ShouldFail_WhenStatusCannotTransitionToNewStatus()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var initialStatus = workflow.Statuses.First(x => x.IsInitial);
        var unavailableStatus = workflow.Statuses.First(x => !initialStatus.CanTransitionTo(x.Id));

        var task = Task.Create(1, Guid.NewGuid(), "title", "desc", initialStatus.Id);

        var result = task.UpdateStatus(unavailableStatus.Id, workflow);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatusId_WhenStatusCanTransitionToNewStatus()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var initialStatus = workflow.Statuses.First(x => x.IsInitial);
        var availableStatus = workflow.Statuses.First(x => initialStatus.CanTransitionTo(x.Id));

        var task = Task.Create(1, Guid.NewGuid(), "title", "desc", initialStatus.Id);

        var result = task.UpdateStatus(availableStatus.Id, workflow);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            task.StatusId.Should().Be(availableStatus.Id);
        }
    }
}
