﻿namespace Shared.Dtos;

public record CreateRoleDto<TPermissions>(string Name, TPermissions Permissions)
    where TPermissions : struct, Enum;

public record UpdateRoleNameDto(string Name);
public record UpdateMemberRoleDto(Guid RoleId);
