namespace Shared.Enums;

[Flags]
public enum OrganizationPermissions
{
    None = 0,
    CreateProjects = 1 << 0, // 1
    InviteMembers = 1 << 1, // 2
    RemoveMembers = 1 << 2, // 4
    ManageRoles = 1 << 3, // 8
}

[Flags]
public enum ProjectPermissions
{
    None = 0,
    Tasks = 1 << 0, // 1
    Members = 1 << 1, // 2
    Workflows = 1 << 2, // 4
}