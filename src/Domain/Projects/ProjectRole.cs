using Domain.Common;
using Shared.Enums;

namespace Domain.Projects;

public class ProjectRole : Role<ProjectPermissions>
{
    public Guid ProjectId { get; private set; }

    public ProjectRole(string name, Guid projectId, ProjectPermissions permissions) 
        : this(name, projectId, permissions, RoleType.Custom)
    {
    }

    private ProjectRole(string name, Guid projectId, ProjectPermissions permissions, RoleType type)
        : base(name, permissions, type)
    {
        ProjectId = projectId;
    }

    public static ProjectRole[] CreateDefaultRoles(Guid projectId)
    {
        return new ProjectRole[]
        {
            new("Administrator", projectId, EnumHelpers.GetAllFlags<ProjectPermissions>(), RoleType.Admin),
            new("Read-Only", projectId, ProjectPermissions.None, RoleType.ReadOnly),
        };
    }
}
