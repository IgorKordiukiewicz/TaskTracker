import type { TaskPriority } from "../enums";

export interface TasksVM {
    tasks: TaskVM[];
    allTaskStatuses: TaskStatusDetailsVM[];
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

export interface TaskCommentsVM {
    comments: TaskCommentVM[];
}

export interface TaskCommentVM {
    content: string;
    authorId: string;
    authorName: string;
    createdAt: Date;
}