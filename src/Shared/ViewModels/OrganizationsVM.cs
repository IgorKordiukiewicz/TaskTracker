using Shared.Enums;

namespace Shared.ViewModels;

public record OrganizationInvitationsVM(IReadOnlyList<OrganizationInvitationVM> Invitations);
public record OrganizationInvitationVM(Guid Id, string UserEmail, OrganizationInvitationState State, DateTime CreatedAt, DateTime? FinalizedAt);

public record OrganizationsForUserVM(IReadOnlyList<OrganizationForUserVM> Organizations);

public record OrganizationForUserVM
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public record UserOrganizationInvitationsVM(IReadOnlyList<UserOrganizationInvitationVM> Invitations);
public record UserOrganizationInvitationVM(Guid Id, string OrganizationName);

public record OrganizationSettingsVM(string Name, Guid OwnerId);

public record UserOrganizationPermissionsVM(OrganizationPermissions Permissions);


