export enum OrganizationPermissions {
    None = 0,
    EditProjects = 1 << 0, // 1
    EditMembers = 1 << 1, // 2
    EditRoles = 1 << 2, // 4
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