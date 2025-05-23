namespace Application.Models.Dtos;

public record CreateProjectDto(string Name);

public record CreateProjectInvitationDto(Guid UserId, int? ExpirationDays = null);

public record AddProjectMemberDto(Guid UserId);
public record RemoveProjectMemberDto(Guid MemberId);

public record UpdateProjectNameDto(string Name);
