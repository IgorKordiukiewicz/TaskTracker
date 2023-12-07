using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ViewModels;

public record UserVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required IReadOnlyDictionary<Guid, OrganizationPermissions> PermissionsByOrganization { get; init; }
    public required IReadOnlyDictionary<Guid, ProjectPermissions> PermissionsByProject { get; init; }
}

public record UsersSearchVM(IReadOnlyList<UserSearchVM> Users);

public record UserSearchVM
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
}
