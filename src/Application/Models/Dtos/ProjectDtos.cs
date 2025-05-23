namespace Application.Models.Dtos;

public record CreateProjectDto(string Name);

public record CreateProjectInvitationDto(Guid UserId, int? ExpirationDays = null);

public record AddProjectMemberDto(Guid UserId);
public record RemoveProjectMemberDto(Guid MemberId);

public record UpdateProjectNameDto(string Name);

public record CreateRoleDto(string Name, ProjectPermissions Permissions);
public record DeleteRoleDto(Guid RoleId);

public record UpdateRoleNameDto(Guid RoleId, string Name);
public record UpdateRolePermissionsDto(Guid RoleId, ProjectPermissions Permissions);
public record UpdateMemberRoleDto(Guid MemberId, Guid RoleId);

