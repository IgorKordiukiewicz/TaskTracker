using Infrastructure.Models;

namespace Application.Common;

public static class TasksBoardLayoutExtensions
{
    public static void CreateTask(this TasksBoardLayout layout, Guid taskId, Guid statusId)
    {
        var column = layout.Columns.Single(x => x.StatusId == statusId);
        column.TasksIds.Add(taskId);
    }

    public static void DeleteTask(this TasksBoardLayout layout, Guid taskId)
    {
        var column = layout.Columns.Single(x => x.TasksIds.Contains(taskId));
        column.TasksIds.Remove(taskId);
    }

    public static void UpdateTaskStatus(this TasksBoardLayout layout, Guid taskId, Guid newStatusId)
    {
        var oldColumn = layout.Columns.Single(x => x.TasksIds.Contains(taskId));
        oldColumn.TasksIds.Remove(taskId);
        var newColumn = layout.Columns.Single(x => x.StatusId == newStatusId);
        newColumn.TasksIds.Add(taskId);
    }

    public static void Initialize(this TasksBoardLayout layout, IEnumerable<Guid> statusesIds)
    {
        layout.Columns = statusesIds.Select(x => new TasksBoardColumn()
        {
            StatusId = x
        }).ToArray();
    }

    public static void AddStatus(this TasksBoardLayout layout, Guid statusId)
    {
        layout.Columns = layout.Columns.Append(new()
        {
            StatusId = statusId,
        }).ToArray();
    }

    public static void DeleteStatus(this TasksBoardLayout layout, Guid statusId)
    {
        layout.Columns = layout.Columns
            .Where(x => x.StatusId != statusId)
            .ToArray();
    }

    public static void Update(this TasksBoardLayout layout, IReadOnlyCollection<TasksBoardColumn> columns)
    {
        layout.Columns = [.. columns];
    }
}
