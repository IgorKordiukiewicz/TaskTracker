using Application.Common;
using Infrastructure.Models;

namespace UnitTests.Application;

public class TasksBoardLayoutExtensionsTests
{
    [Fact]
    public void CreateTask_ShouldAddTaskToStatusColumn()
    {
        var sut = GetLayout();
        var taskId = Guid.NewGuid();
        var statusId = sut.Columns[0].StatusId;

        sut.CreateTask(taskId, statusId);

        sut.Columns.Any(x => x.StatusId == statusId && x.TasksIds.Any(xx => xx == taskId)).Should().BeTrue();
    }

    [Fact]
    public void DeleteTask_ShouldRemoveTaskFromStatusColumn()
    {
        var sut = GetLayout();
        var taskId = sut.Columns[0].TasksIds[0];

        sut.DeleteTask(taskId);

        sut.Columns.Any(x => x.TasksIds.Any(xx => xx == taskId)).Should().BeFalse();
    }

    [Fact]
    public void UpdateTaskStatus_ShouldMoveTaskToAnotherColumn()
    {
        var sut = GetLayout();
        var taskId = sut.Columns[0].TasksIds[0];
        var newStatusId = sut.Columns[1].StatusId;

        sut.UpdateTaskStatus(taskId, newStatusId);

        sut.Columns.Any(x => x.StatusId == newStatusId && x.TasksIds.Any(xx => xx == taskId)).Should().BeTrue();
    }

    [Fact]
    public void Initialize_ShouldCreateEmptyStatusColumns()
    {
        var sut = new TasksBoardLayout()
        {
            ProjectId = Guid.NewGuid()
        };

        var statusesIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

        sut.Initialize(statusesIds);

        using(new AssertionScope())
        {
            sut.Columns.Select(x => x.StatusId).Should().BeEquivalentTo(statusesIds);
            sut.Columns.SelectMany(x => x.TasksIds).Should().BeEmpty();
        }
    }

    [Fact]
    public void AddStatus_ShouldAddNewEmptyColumn()
    {
        var sut = GetLayout();
        var statusId = Guid.NewGuid();

        sut.AddStatus(statusId);

        using(new AssertionScope())
        {
            var column = sut.Columns.FirstOrDefault(x => x.StatusId == statusId);
            column.Should().NotBeNull();
            column!.TasksIds.Should().BeEmpty();
        }
    }

    [Fact]
    public void DeleteStatus_ShouldDeleteColumn()
    {
        var sut = GetLayout();
        var statusId = sut.Columns[0].StatusId;

        sut.DeleteStatus(statusId);

        sut.Columns.Any(x => x.StatusId == statusId).Should().BeFalse();
    }

    [Fact]
    public void Update_ShouldSetColumnsToGivenParam()
    {
        var sut = GetLayout();
        var newColumns = new List<TasksBoardColumn>()
        {
            new()
            {
                StatusId = Guid.NewGuid(),
                TasksIds = [Guid.NewGuid()]
            }
        };

        sut.Update(newColumns);

        sut.Columns.Should().BeEquivalentTo(newColumns);
    }

    private static TasksBoardLayout GetLayout()
    {
        return new TasksBoardLayout()
        {
            ProjectId = Guid.NewGuid(),
            Columns =
            [
                new TasksBoardColumn()
                {
                    StatusId = Guid.NewGuid(),
                    TasksIds = [Guid.NewGuid(), Guid.NewGuid()],
                },
                new TasksBoardColumn()
                {
                    StatusId = Guid.NewGuid(),
                    TasksIds = [Guid.NewGuid()],
                },
            ]
        };
    }
}
