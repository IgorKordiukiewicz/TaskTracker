using Domain.Errors;
using FluentResults;

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

    public static Result AddRole<TPermissions, TRole>(this List<TRole> roles, TRole newRole)
        where TRole : Role<TPermissions>
        where TPermissions : struct, Enum
    {
        if(roles.Any(x => x.Name.ToLower() == newRole.Name.ToLower()))
        {
            return Result.Fail(new DomainError("Role with this name already exists."));
        }

        roles.Add(newRole); // check if it works
        return Result.Ok();
    }
}