using Domain.Errors;
using FluentResults;

namespace Domain.Common;

public class RolesManager<TRole, TPermissions>
    where TRole : Role<TPermissions>
    where TPermissions : struct, Enum
{
    private readonly List<TRole> _roles;
    private readonly Func<string, TPermissions, TRole> _createNewRoleFunc;

    public RolesManager(List<TRole> roles, Func<string, TPermissions, TRole> createNewRoleFunc)
    {
        _roles = roles;
        _createNewRoleFunc = createNewRoleFunc;
    }

    public Guid GetAdminRoleId()
        => _roles.Single(x => x.Type == RoleType.Admin).Id;

    public Guid GetReadOnlyRoleId()
        => _roles.Single(x => x.Type == RoleType.ReadOnly).Id;

    public Result AddRole(string name, TPermissions permissions)
    {
        if (IsNameTaken(name))
        {
            return Result.Fail(new DomainError("Role with this name already exists."));
        }

        var newRole = _createNewRoleFunc(name, permissions);
        _roles.Add(newRole);
        return Result.Ok();
    }

    public Result UpdateRoleName(Guid roleId, string newName)
    {
        var role = _roles.FirstOrDefault(x => x.Id == roleId);
        if (role is null)
        {
            return Result.Fail(new DomainError("Role wih this ID does not exist."));
        }

        if(!role.IsModifiable())
        {
            return Result.Fail(new DomainError("This role can not be modified."));
        }

        if (IsNameTaken(newName))
        {
            return Result.Fail(new DomainError("Role with this name already exists."));
        }

        role.UpdateName(newName);
        return Result.Ok();
    }

    public Result DeleteRole(Guid roleId)
    {
        var role = _roles.FirstOrDefault(x => x.Id == roleId);
        if(role is null)
        {
            return Result.Fail(new DomainError("Role wih this ID does not exist."));
        }

        if(!role.IsModifiable())
        {
            return Result.Fail(new DomainError("This role can not be deleted."));
        }

        _roles.Remove(role);
        return Result.Ok();
    }

    private bool IsNameTaken(string name)
        => _roles.Any(x => x.Name.ToLower() == name.ToLower());
}