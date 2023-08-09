namespace Shared.Dtos;

public record CreateProjectDto(string Name);

public record AddProjectMemberDto(Guid UserId);
