import type { AttachmentType, TaskPriority, TaskProperty } from "../enums";

export interface TasksVM {
    tasks: TaskVM[];
    allTaskStatuses: TaskStatusDetailsVM[];
    boardColumns: TaskBoardColumnVM[];
}

export interface TaskVM {
    id: string;
    shortId: number;
    title: string;
    description: string;
    assigneeId?: string;
    priority: TaskPriority;
    status: TaskStatusVM;
    possibleNextStatuses: TaskStatusVM[];
    totalTimeLogged: number;
    estimatedTime?: number;
    commentsCount: number;
}

export interface TaskStatusVM {
    id: string;
    name: string;
}

export interface TaskStatusDetailsVM {
    id: string;
    name: string;
    displayOrder: number;
}

export interface TaskBoardColumnVM {
    statusId: string;
    statusName: string;
    possibleNextStatuses: string[];
    tasksIds: string[];
}

export interface TaskCommentsVM {
    comments: TaskCommentVM[];
}

export interface TaskCommentVM {
    content: string;
    authorId: string;
    authorName: string;
    createdAt: Date;
}

export interface TaskActivitiesVM {
    activities: TaskActivityVM[];
}

export interface TaskActivityVM {
    property: TaskProperty;
    oldValue: string;
    newValue: string;
    occurredAt: Date;
}

export interface TaskRelationshipsVM {
    parent?: TaskRelationshipsParentVM;
    childrenHierarchy: TaskHierarchyVM[];
}

export interface TaskRelationshipsParentVM {
    id: string;
    title: string;
    shortId: number;
}

export interface TaskHierarchyVM {
    taskId: string;
    taskTitle: string;
    taskShortId: number;
    children: TaskHierarchyVM[];
}

export interface TaskAvailableChildrenVM {
    tasks: TaskAvailableChildVM[];
}

export interface TaskAvailableChildVM {
    id: string;
    shortId: number;
    title: string;
}

export interface TaskAttachmentsVM {
    attachments: TaskAttachmentVM[];
}

export interface TaskAttachmentVM {
    name: string;
    bytesLength: number;
    type: AttachmentType;
    downloadUrl: string;
}