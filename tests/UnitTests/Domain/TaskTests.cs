﻿using Domain.Workflows;
using Task = Domain.Tasks.Task;

namespace UnitTests.Domain;

public class TaskTests
{
    [Fact]
    public void Create_ShouldCreateTask_WithGivenParametersAndCreatedActivity()
    {
        var shortId = 1;
        var projectId = Guid.NewGuid();
        var title = "Title";
        var description = "Description";
        var statusId = Guid.NewGuid();

        var result = Task.Create(shortId, projectId, DateTime.Now, title, description, statusId);

        using(new AssertionScope())
        {
            result.Id.Should().NotBeEmpty();
            result.ShortId.Should().Be(shortId);
            result.ProjectId.Should().Be(projectId);
            result.Title.Should().Be(title);
            result.Description.Should().Be(description);
            result.StatusId.Should().Be(statusId);
            result.Activities.Count().Should().Be(1);
        }
    }

    [Fact]
    public void UpdateStatus_ShouldFail_WhenStatusCannotTransitionToNewStatus()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var initialStatus = workflow.Statuses.First(x => x.Initial);
        var availableStatuses = workflow.Transitions.Where(x => x.FromStatusId == initialStatus.Id).Select(x => x.ToStatusId);
        var unavailableStatus = workflow.Statuses.First(x => !availableStatuses.Contains(x.Id));

        var task = Task.Create(1, Guid.NewGuid(), DateTime.Now, "title", "desc", initialStatus.Id);

        var result = task.UpdateStatus(unavailableStatus.Id, workflow, DateTime.Now);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatusId_WhenStatusCanTransitionToNewStatus()
    {
        var workflow = Workflow.Create(Guid.NewGuid());
        var initialStatus = workflow.Statuses.First(x => x.Initial);
        var availableStatusId = workflow.Transitions.First(x => x.FromStatusId == initialStatus.Id).ToStatusId;

        var task = Task.Create(1, Guid.NewGuid(), DateTime.Now, "title", "desc", initialStatus.Id);

        var result = task.UpdateStatus(availableStatusId, workflow, DateTime.Now);

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

        task.UpdateAssignee(assigneeId, DateTime.Now);

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
        task.UpdateAssignee(Guid.NewGuid(), DateTime.Now);
        var assigneeIdBefore = task.AssigneeId;

        task.Unassign(DateTime.Now);

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

        task.UpdatePriority(newPriority, DateTime.Now);

        using(new AssertionScope())
        {
            task.Priority.Should().Be(newPriority);
            task.Activities.Any(x => x.Property == TaskProperty.Priority).Should().BeTrue();
        }
    }

    [Fact]
    public void UpdateTitle_ShouldUpdateTitleAndAddActivity()
    {
        var task = CreateDefaultTask();
        var newTitle = task.Title + "A";

        task.UpdateTitle(newTitle, DateTime.Now);

        using(new AssertionScope())
        {
            task.Title.Should().Be(newTitle);
            task.Activities.Any(x => x.Property == TaskProperty.Title).Should().BeTrue();
        }
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescriptionAndAddActivity()
    {
        var task = CreateDefaultTask();
        var newDescription = task.Description + "A";

        task.UpdateDescription(newDescription, DateTime.Now);

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

    [Fact]
    public void UpdateEstimatedTime_ShouldUpdateValue_WhenMinutesArePositive()
    {
        var task = CreateDefaultTask();
        const int expected = 10;
        
        task.UpdateEstimatedTime(expected);

        task.EstimatedTime.Should().Be(expected);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(0)]
    public void UpdateEstimatedTime_ShouldSetValueToNull_WhenMinutesAreZeroOrNegative(int minutes)
    {
        var task = CreateDefaultTask();
        
        task.UpdateEstimatedTime(minutes);

        task.EstimatedTime.Should().BeNull();
    }
    
    private static Task CreateDefaultTask()
        => Task.Create(1, Guid.NewGuid(), DateTime.Now, "title", "desc", Guid.NewGuid());
}
