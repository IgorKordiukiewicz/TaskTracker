export enum OrganizationPermissions {
    None = 0,
    EditProjects = 1 << 0, // 1
    EditMembers = 1 << 1, // 2
    EditRoles = 1 << 2, // 4
    EditOrganization = 1 << 3 // 8
}

export enum ProjectPermissions {
    None = 0,
    EditTasks = 1 << 0, // 1
    EditMembers = 1 << 1, // 2
    EditRoles = 1 << 2, // 4
    EditProject = 1 << 3 // 8
}

export enum OrganizationInvitationState
{
    Pending,
    Accepted,
    Declined,
    Canceled
}

export enum TaskPriority {
    Low,
    Normal,
    High,
    Urgent
}

export enum TaskProperty
{
    Description,
    Status,
    Assignee,
    Priority
}

export enum TaskStatusDeletionEligibility 
{
    Eligible,
    InUse,
    Initial
}