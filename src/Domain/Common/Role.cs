namespace Domain.Common;

public abstract class Role<TPermissions> : Entity 
    where TPermissions: struct, Enum
{
    public string Name { get; private set; }
    public bool Modifiable { get; private init; }

    public TPermissions Permissions { get; private set; }

    public Role(string name, TPermissions permissions, bool modifiable = true)
        : base(Guid.NewGuid())
    {
        Name = name;
        Permissions = permissions;
        Modifiable = modifiable;
    }

    public bool HasPermission(TPermissions flag) // extract to Shared/FlagsHelpers or sth so it can also be used in Client code
    {
        var (permissionsValue, flagValue) = GetPermissionsValues(flag);
        return (permissionsValue & flagValue) == flagValue;
    }

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


    public bool IsAdminRole() => Name == "Administrator";
    public bool IsReadOnlyRole() => Name == "Read-Only";
}