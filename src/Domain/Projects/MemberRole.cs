namespace Domain.Projects;

public enum RoleType
{
    Custom,
    Admin,
    ReadOnly,
    Owner
}

public class MemberRole : Entity
{
    public string Name { get; set; }
    public RoleType Type { get; private set; }
    public Guid ProjectId { get; private set; }
    public ProjectPermissions Permissions { get; set; }

    public MemberRole(string name, Guid projectId, ProjectPermissions permissions) 
        : this(name, projectId, permissions, RoleType.Custom)
    {
    }

    private MemberRole(string name, Guid projectId, ProjectPermissions permissions, RoleType type)
        : base(Guid.NewGuid())
    {
        ProjectId = projectId;
        Name = name;
        Permissions = permissions;
        Type = type;
    }

    public bool HasPermission(ProjectPermissions flag)
        => Permissions.HasFlag(flag);

    public bool IsModifiable()
        => Type == RoleType.Custom;

    public bool IsOwner()
        => Type == RoleType.Owner;

    public static MemberRole[] CreateDefaultRoles(Guid projectId)
    {
        return
        [
            new("Owner", projectId, EnumHelpers.GetAllFlags<ProjectPermissions>(), RoleType.Owner),
            new("Administrator", projectId, EnumHelpers.GetAllFlags<ProjectPermissions>(), RoleType.Admin),
            new("Read-Only", projectId, ProjectPermissions.None, RoleType.ReadOnly),
        ];
    }
}
