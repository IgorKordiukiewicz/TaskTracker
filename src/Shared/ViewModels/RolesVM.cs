﻿namespace Shared.ViewModels;

public record RolesVM<TPermissions>(IReadOnlyList<RoleVM<TPermissions>> Roles) 
    where TPermissions : struct, Enum;

public record RoleVM<TPermissions>
    where TPermissions : struct, Enum
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required TPermissions Permissions { get; init; }
    public required bool Modifiable { get; init; }
}