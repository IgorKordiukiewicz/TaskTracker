namespace Shared.ViewModels;

public record UserOrganizationInvitations(IReadOnlyList<UserOrganizationInvitationVM> Invitations);
public record UserOrganizationInvitationVM(Guid Id, string OrganizationName);
