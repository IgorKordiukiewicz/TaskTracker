namespace Application.Models.Dtos;

public record CreateOrganizationDto(string Name);

public record CreateOrganizationInvitationDto(Guid UserId);

public record RemoveOrganizationMemberDto(Guid MemberId);

public record UpdateOrganizationNameDto(string Name);
