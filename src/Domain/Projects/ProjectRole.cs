using Domain.Common;
using Shared.Enums;

namespace Domain.Projects;

public class ProjectRole : Role<ProjectPermissions>
{
    public Guid ProjectId { get; private set; }

    public ProjectRole(string name, Guid projectId, ProjectPermissions permissions) 
        : base(name, permissions)
    {
        ProjectId = projectId;
    }
}
