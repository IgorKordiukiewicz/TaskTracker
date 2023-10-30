using Shared.Enums;

namespace Shared.ViewModels;

public record OrganizationInvitationsVM(IReadOnlyList<OrganizationInvitationVM> Invitations);
public record OrganizationInvitationVM(Guid Id, string UserEmail, OrganizationInvitationState State);
