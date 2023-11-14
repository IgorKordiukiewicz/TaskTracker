using Shared.Enums;

namespace Shared.ViewModels;

public record UserVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required IReadOnlyDictionary<Guid, OrganizationPermissions> PermissionsByOrganization { get; init; }
    public required IReadOnlyDictionary<Guid, ProjectPermissions> PermissionsByProject { get; init; }
}
