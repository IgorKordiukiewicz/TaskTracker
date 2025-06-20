export enum ProjectPermissions {
    None = 0,
    EditTasks = 1 << 0, // 1
    EditMembers = 1 << 1, // 2
    EditRoles = 1 << 2, // 4
    EditProject = 1 << 3 // 8
}

export enum ProjectInvitationState
{
    Pending,
    Accepted,
    Declined,
    Canceled,
    Expired
}

export const allInvitationStates = [
    { key: ProjectInvitationState.Pending, name: ProjectInvitationState[ProjectInvitationState.Pending] },
    { key: ProjectInvitationState.Accepted, name: ProjectInvitationState[ProjectInvitationState.Accepted] },
    { key: ProjectInvitationState.Declined, name: ProjectInvitationState[ProjectInvitationState.Declined] },
    { key: ProjectInvitationState.Canceled, name: ProjectInvitationState[ProjectInvitationState.Canceled] },
]

export enum TaskPriority {
    Low,
    Normal,
    High,
    Urgent
}

export const allTaskPriorities = [
    { key: TaskPriority.Low, name: TaskPriority[TaskPriority.Low] },
    { key: TaskPriority.Normal, name: TaskPriority[TaskPriority.Normal] },
    { key: TaskPriority.High, name: TaskPriority[TaskPriority.High] },
    { key: TaskPriority.Urgent, name: TaskPriority[TaskPriority.Urgent] },
]

export enum TaskProperty
{
    Description,
    Status,
    Assignee,
    Priority,
    Title,
    Creation
}

export enum TaskStatusDeletionEligibility 
{
    Eligible,
    InUse,
    Initial
}

export enum AttachmentType
{
    Document,
    Image
}