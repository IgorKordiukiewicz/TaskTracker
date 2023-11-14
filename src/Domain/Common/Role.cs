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
    public string Name { get; private set; }
    public RoleType Type { get; private set; }

    public TPermissions Permissions { get; private set; }

    public Role(string name, TPermissions permissions, RoleType type = RoleType.Custom)
        : base(Guid.NewGuid())
    {
        Name = name;
        Permissions = permissions;
        Type = type;
    }

    public bool HasPermission(TPermissions flag)
        => Permissions.HasFlag(flag);

    public void AddPermission(TPermissions flag)
    {
        var (permissionsValue, flagValue) = GetPermissionsValues(flag);
        Permissions = (TPermissions)Enum.ToObject(typeof(TPermissions), permissionsValue | flagValue);
    }

    public void RemovePermission(TPermissions flag)
    {
        var (permissionsValue, flagValue) = GetPermissionsValues(flag);
        Permissions = (TPermissions)Enum.ToObject(typeof(TPermissions), permissionsValue & ~flagValue);
    }

    private (int Permissions, int Flag) GetPermissionsValues(TPermissions flag)
        => (Convert.ToInt32(Permissions), Convert.ToInt32(flag));
}