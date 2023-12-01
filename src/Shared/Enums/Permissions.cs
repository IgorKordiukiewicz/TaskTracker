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
    AddMembers = 1 << 0, // 1
    RemoveMembers = 1 << 1, // 2
    CreateTasks = 1 << 2, // 4
    ModifyTasks = 1 << 3, // 8
    TransitionTasks = 1 << 4, // 16
    AssignTasks = 1 << 5, // 32
    AddComments = 1 << 6, // 64
    ManageWorkflows = 1 << 7, // 128
    ManageProject = 1 << 8, // 256
    ManageRoles = 1 << 9, // 512
}