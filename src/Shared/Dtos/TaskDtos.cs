namespace Shared.Dtos;

public record CreateTaskDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}
