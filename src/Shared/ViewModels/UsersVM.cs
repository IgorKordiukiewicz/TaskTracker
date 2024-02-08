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
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
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

public record UsersPresentationDataVM(IReadOnlyList<UserPresentationDataVM> Data);
public record UserPresentationDataVM(Guid UserId, string AvatarColor);