namespace Application.Models.Dtos;

public record CreateRoleDto<TPermissions>(string Name, TPermissions Permissions)
    where TPermissions : struct, Enum;

public record UpdateRoleNameDto(Guid RoleId, string Name);
public record UpdateRolePermissionsDto<TPermissions>(Guid RoleId, TPermissions Permissions)
    where TPermissions : struct, Enum;

public record DeleteRoleDto(Guid RoleId);

public record UpdateMemberRoleDto(Guid MemberId, Guid RoleId);
