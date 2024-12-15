namespace Infrastructure.Models;

public class TasksBoardLayout
{
    public required Guid ProjectId { get; init; }
    public TasksBoardColumn[] Columns { get; set; } = [];
}

public record TasksBoardColumn
{
    public required Guid StatusId { get; init; }
    public List<Guid> TasksIds { get; init; } = [];
}