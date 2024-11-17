namespace Domain.Common;

public interface IHasRole
{
    Guid Id { get; }
    Guid UserId { get; }
    Guid RoleId { get; }
    void UpdateRole(Guid roleId);
}

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

    public Guid? GetOwnerRoleId()
        => _roles.FirstOrDefault(x => x.Type == RoleType.Owner)?.Id;

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

        role.Name = newName;
        return Result.Ok();
    }

    public Result DeleteRole(Guid roleId, IReadOnlyCollection<IHasRole> members)
    {
        var role = _roles.FirstOrDefault(x => x.Id == roleId);
        if(role is null)
        {
            return Result.Fail(new DomainError("Role wih this ID does not exist."));
        }

        if(!role.IsModifiable() || members.Any(x => x.RoleId == roleId))
        {
            return Result.Fail(new DomainError("This role can not be deleted."));
        }

        _roles.Remove(role);
        return Result.Ok();
    }

    public Result UpdateMemberRole(Guid memberId, Guid roleId, IReadOnlyCollection<IHasRole> members)
    {
        var member = members.FirstOrDefault(x => x.Id == memberId);
        if(member is null)
        {
            return Result.Fail(new DomainError("Member with this ID does not exist."));
        }

        var currentRole = _roles.First(x => x.Id == member.RoleId);
        if (currentRole.Type == RoleType.Owner)
        {
            return Result.Fail(new DomainError("Member's current role can not be assigned from."));
        }

        var role = _roles.FirstOrDefault(x => x.Id == roleId);
        if(role is null)
        {
            return Result.Fail(new DomainError("Role wih this ID does not exist."));
        }

        if(role.Type == RoleType.Owner)
        {
            return Result.Fail(new DomainError("This role can not be assigned."));
        }

        member.UpdateRole(roleId);
        return Result.Ok();
    }

    public Result UpdateRolePermissions(Guid roleId, TPermissions permissions)
    {
        var role = _roles.FirstOrDefault(x => x.Id == roleId);
        if(role is null)
        {
            return Result.Fail(new DomainError("Role wih this ID does not exist."));
        }

        if(!role.IsModifiable())
        {
            return Result.Fail(new DomainError("This role can not be modified."));
        }

        role.Permissions = permissions;
        return Result.Ok();
    }

    private bool IsNameTaken(string name)
        => _roles.Any(x => x.Name.ToLower() == name.ToLower());
}