using Microsoft.AspNetCore.Authorization;
using Shared.Enums;

namespace Shared.Authorization;

public class OrganizationMemberRequirement : IAuthorizationRequirement
{
    public OrganizationPermissions? Permissions { get; set; }

    public OrganizationMemberRequirement(OrganizationPermissions? permissions = null)
    {
        Permissions = permissions;
    }
}

public class ProjectMemberRequirement : IAuthorizationRequirement
{
    public ProjectPermissions? Permissions { get; set; }

    public ProjectMemberRequirement(ProjectPermissions? permissions = null)
    {
        Permissions = permissions;
    }
}
