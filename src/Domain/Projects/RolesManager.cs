namespace Domain.Projects;

public class RolesManager
{
    private readonly List<MemberRole> _roles;
    private readonly Guid _projectId;

    public RolesManager(List<MemberRole> roles, Guid projectId)
    {
        _roles = roles;
        _projectId = projectId;
    }

    public Guid? GetOwnerRoleId()
        => _roles.FirstOrDefault(x => x.Type == RoleType.Owner)?.Id;

    public Guid GetAdminRoleId()
        => _roles.Single(x => x.Type == RoleType.Admin).Id;

    public Guid GetReadOnlyRoleId()
        => _roles.Single(x => x.Type == RoleType.ReadOnly).Id;

    public Result AddRole(string name, ProjectPermissions permissions)
    {
        if (IsNameTaken(name))
        {
            return Result.Fail(new DomainError("Role with this name already exists."));
        }

        _roles.Add(new MemberRole(name, _projectId, permissions));

        return Result.Ok();
    }

    public Result UpdateRoleName(Guid roleId, string newName)
    {
        var role = _roles.FirstOrDefault(x => x.Id == roleId);
        if (role is null)
        {
            return Result.Fail(new DomainError("Role wih this ID does not exist."));
        }

        if (!role.IsModifiable())
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

    public Result DeleteRole(Guid roleId, IReadOnlyCollection<ProjectMember> members)
    {
        var role = _roles.FirstOrDefault(x => x.Id == roleId);
        if (role is null)
        {
            return Result.Fail(new DomainError("Role wih this ID does not exist."));
        }

        if (!role.IsModifiable() || members.Any(x => x.RoleId == roleId))
        {
            return Result.Fail(new DomainError("This role can not be deleted."));
        }

        _roles.Remove(role);
        return Result.Ok();
    }

    public Result UpdateMemberRole(Guid memberId, Guid roleId, IReadOnlyCollection<ProjectMember> members)
    {
        var member = members.FirstOrDefault(x => x.Id == memberId);
        if (member is null)
        {
            return Result.Fail(new DomainError("Member with this ID does not exist."));
        }

        var role = _roles.FirstOrDefault(x => x.Id == roleId);
        if (role is null)
        {
            return Result.Fail(new DomainError("Role wih this ID does not exist."));
        }

        var currentRole = _roles.First(x => x.Id == member.RoleId);
        if (currentRole.Type == RoleType.Owner)
        {
            return Result.Fail(new DomainError("Member's current role can not be assigned from."));
        }

        if (role.Type == RoleType.Owner)
        {
            return Result.Fail(new DomainError("This role can not be assigned."));
        }

        member.UpdateRole(roleId);
        return Result.Ok();
    }

    public Result UpdateRolePermissions(Guid roleId, ProjectPermissions permissions)
    {
        var role = _roles.FirstOrDefault(x => x.Id == roleId);
        if (role is null)
        {
            return Result.Fail(new DomainError("Role wih this ID does not exist."));
        }

        if (!role.IsModifiable())
        {
            return Result.Fail(new DomainError("This role can not be modified."));
        }

        role.Permissions = permissions;
        return Result.Ok();
    }

    private bool IsNameTaken(string name)
        => _roles.Any(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
}