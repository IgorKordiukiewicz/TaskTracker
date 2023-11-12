namespace Domain.Common;

public static class RolesExtensions
{
    public static Guid GetAdminRoleId<TPermissions>(this IEnumerable<Role<TPermissions>> roles)
        where TPermissions : struct, Enum
    {
        return roles.Single(x => x.Type == RoleType.Admin).Id;
    }

    public static Guid GetReadOnlyRoleId<TPermissions>(this IEnumerable<Role<TPermissions>> roles)
        where TPermissions : struct, Enum
    {
        return roles.Single(x => x.Type == RoleType.ReadOnly).Id;
    }
}