namespace Domain.Common;

public enum RoleType
{
    Custom,
    Admin,
    ReadOnly,
}

public abstract class Role<TPermissions> : Entity 
    where TPermissions: struct, Enum
{
    public string Name { get; set; }
    public RoleType Type { get; private set; }

    public TPermissions Permissions { get; set; }

    public Role(string name, TPermissions permissions, RoleType type = RoleType.Custom)
        : base(Guid.NewGuid())
    {
        Name = name;
        Permissions = permissions;
        Type = type;
    }

    public bool HasPermission(TPermissions flag)
        => Permissions.HasFlag(flag);

    public bool IsModifiable()
        => Type == RoleType.Custom;
}