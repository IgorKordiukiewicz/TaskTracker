export enum ProjectPermissions {
    None = 0,
    EditTasks = 1 << 0, // 1
    EditMembers = 1 << 1, // 2
    EditRoles = 1 << 2, // 4
    EditProject = 1 << 3 // 8
}

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