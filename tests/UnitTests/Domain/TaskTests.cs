using Domain.Workflows;
using Shared.Enums;
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
    public void UpdateStatus_ShouldFail_WhenStatusCannotTransitionToNewStatus()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var initialStatus = workflow.Statuses.First(x => x.Initial);
        var availableStatuses = workflow.Transitions.Where(x => x.FromStatusId == initialStatus.Id).Select(x => x.ToStatusId);
        var unavailableStatus = workflow.Statuses.First(x => !availableStatuses.Contains(x.Id));

        var task = Task.Create(1, Guid.NewGuid(), "title", "desc", initialStatus.Id);

        var result = task.UpdateStatus(unavailableStatus.Id, workflow);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatusId_WhenStatusCanTransitionToNewStatus()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var initialStatus = workflow.Statuses.First(x => x.Initial);
        var availableStatusId = workflow.Transitions.First(x => x.FromStatusId == initialStatus.Id).ToStatusId;

        var task = Task.Create(1, Guid.NewGuid(), "title", "desc", initialStatus.Id);

        var result = task.UpdateStatus(availableStatusId, workflow);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            task.StatusId.Should().Be(availableStatusId);
            task.Activities.Any(x => x.Property == TaskProperty.Status).Should().BeTrue();
        }
    }

    [Fact]
    public void UpdateAssignee_ShouldUpdateAssigneeId()
    {
        var task = CreateDefaultTask();
        var assigneeId = Guid.NewGuid();

        task.UpdateAssignee(assigneeId);

        using(new AssertionScope())
        {
            task.AssigneeId.Should().Be(assigneeId);
            task.Activities.Any(x => x.Property == TaskProperty.Assignee).Should().BeTrue();
        }
    }

    [Fact] 
    public void Unassign_ShouldSetAssigneeIdToNull()
    {
        var task = CreateDefaultTask();
        task.UpdateAssignee(Guid.NewGuid());
        var assigneeIdBefore = task.AssigneeId;

        task.Unassign();

        using(new AssertionScope())
        {
            task.AssigneeId.Should().BeNull();
            task.AssigneeId.Should().NotBe(assigneeIdBefore.ToString());
            task.Activities.Count(x => x.Property == TaskProperty.Assignee).Should().Be(2);
        }
    }

    [Fact]
    public void UpdatePriority_ShouldUpdatePriority()
    {
        var task = CreateDefaultTask();
        var priority = task.Priority;
        var newPriority = Enum.GetValues<TaskPriority>().Where(x => x != priority).First();

        task.UpdatePriority(newPriority);

        using(new AssertionScope())
        {
            task.Priority.Should().Be(newPriority);
            task.Activities.Any(x => x.Property == TaskProperty.Priority).Should().BeTrue();
        }
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        var task = CreateDefaultTask();
        var newDescription = task.Description + "A";

        task.UpdateDescription(newDescription);

        using (new AssertionScope())
        {
            task.Description.Should().Be(newDescription);
            task.Activities.Any(x => x.Property == TaskProperty.Description).Should().BeTrue();
        }
    }

    [Fact]
    public void AddComment_ShouldAddComment()
    {
        var task = CreateDefaultTask();

        task.AddComment("abc", Guid.NewGuid(), DateTime.Now);

        task.Comments.Count.Should().Be(1);
    }

    [Fact]
    public void LogTime_ShouldAddNewTimeLog()
    {
        var task = CreateDefaultTask();
        var minutes = 10;
        var userId = Guid.NewGuid();
        var day = DateOnly.FromDayNumber(1);
        
        task.LogTime(minutes, day, userId);

        using (new AssertionScope())
        {
            task.TimeLogs.Count.Should().Be(1);
            task.TimeLogs[0].TaskId.Should().Be(task.Id);
            task.TimeLogs[0].Minutes.Should().Be(minutes);
            task.TimeLogs[0].Day.Should().Be(day);
            task.TimeLogs[0].LoggedBy.Should().Be(userId);
        }
    }

    [Fact]
    public void TotalTimeLogged_ShouldReturnSumOfAllLoggedMinutes()
    {
        var task = CreateDefaultTask();
        var day = DateOnly.FromDayNumber(1);
        var minutes = new int[] { 10, 25 };
        foreach (var m in minutes)
        {
            task.LogTime(m, day, Guid.NewGuid());
        }

        var expected = minutes.Sum();

        task.TotalTimeLogged.Should().Be(expected);
    }
    
    private static Task CreateDefaultTask()
        => Task.Create(1, Guid.NewGuid(), "title", "desc", Guid.NewGuid());
}
