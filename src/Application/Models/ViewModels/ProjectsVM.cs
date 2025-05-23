namespace Application.Models.ViewModels;

public record ProjectsVM(IReadOnlyList<ProjectVM> Projects);

public record ProjectVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MembersCount { get; init; }
    public required int TasksCount { get; init; }
}

public record ProjectSettingsVM(string Name, Guid OwnerId);

public record UserProjectPermissionsVM(ProjectPermissions Permissions, bool IsOwner);