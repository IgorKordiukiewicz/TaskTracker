namespace Shared.Dtos;

public record CreateRoleDto<TPermissions>(string Name, TPermissions Permissions)
    where TPermissions : struct, Enum;
