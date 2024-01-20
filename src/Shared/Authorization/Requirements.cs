using Microsoft.AspNetCore.Authorization;
using Shared.Enums;

namespace Shared.Authorization;

public abstract class MemberRequirement<TPermissions> : IAuthorizationRequirement
    where TPermissions : struct, Enum
{
    public TPermissions? Permissions { get; set; }
}

public class OrganizationMemberRequirement : MemberRequirement<OrganizationPermissions>
{
    public OrganizationMemberRequirement(OrganizationPermissions? permissions = null)
    {
        Permissions = permissions;
    }
}

public class ProjectMemberRequirement : MemberRequirement<ProjectPermissions>
{
    public ProjectMemberRequirement(ProjectPermissions? permissions = null)
    {
        Permissions = permissions;
    }
}
