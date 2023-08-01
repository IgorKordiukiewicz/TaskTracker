namespace Shared.ViewModels;

public record OrganizationInvitationsVM(IReadOnlyList<OrganizationInvitationVM> Invitations);
public record OrganizationInvitationVM(Guid Id, string OrganizationName);
