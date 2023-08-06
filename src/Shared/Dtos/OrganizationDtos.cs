namespace Shared.Dtos;

public record CreateOrganizationDto(string Name, Guid OwnerId);

public record CreateOrganizationInvitationDto(Guid OrganizationId, Guid UserId);

public record UpdateOrganizationInvitationDto(Guid InvitationId);
