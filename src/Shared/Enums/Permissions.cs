namespace Shared.Enums;

[Flags]
public enum OrganizationPermissions
{
    None = 0,
    Projects = 1 << 0, // 1
    Members = 1 << 1, // 2
}

[Flags]
public enum ProjectPermissions
{
    None = 0,
    Tasks = 1 << 0, // 1
    Members = 1 << 1, // 2
    Workflows = 1 << 2, // 4
}