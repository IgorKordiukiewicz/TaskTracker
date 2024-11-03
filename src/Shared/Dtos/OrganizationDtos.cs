namespace Shared.Dtos;

public record CreateOrganizationDto(string Name);

public record CreateOrganizationInvitationDto(Guid UserId);

public record RemoveOrganizationMemberDto(Guid MemberId);
