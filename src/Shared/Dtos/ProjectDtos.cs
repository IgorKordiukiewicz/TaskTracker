namespace Shared.Dtos;

public record CreateProjectDto(Guid OrganizationId, string Name);

public record AddProjectMemberDto(Guid UserId);

public record UpdateProjectNameDto(string Name);
