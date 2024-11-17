namespace Application.Models.ViewModels;

public record ProjectsVM(IReadOnlyList<ProjectVM> Projects);

public record ProjectVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public record ProjectOrganizationVM(Guid OrganizationId);

public record ProjectSettingsVM(string Name);

public record UserProjectPermissionsVM(ProjectPermissions Permissions);