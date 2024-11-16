using Microsoft.AspNetCore.Authorization;
using Shared.Enums;

namespace Shared.Authorization;

public abstract class MemberRequirement<TPermissions> : IAuthorizationRequirement
    where TPermissions : struct, Enum
{
    public TPermissions? Permissions { get; init; }
}

public class OrganizationMemberRequirement : MemberRequirement<OrganizationPermissions>
{
    public bool Owner { get; init; }

    public OrganizationMemberRequirement(OrganizationPermissions? permissions = null, bool owner = false)
    {
        Permissions = permissions;
        Owner = owner;
    }
}

public class ProjectMemberRequirement : MemberRequirement<ProjectPermissions>
{
    public ProjectMemberRequirement(ProjectPermissions? permissions = null)
    {
        Permissions = permissions;
    }
}

public class UserSelfRequirement : IAuthorizationRequirement
{
}