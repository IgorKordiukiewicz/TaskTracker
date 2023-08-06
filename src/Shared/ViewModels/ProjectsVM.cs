namespace Shared.ViewModels;

public record ProjectsVM(IReadOnlyList<ProjectVM> Projects);

public record ProjectVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
