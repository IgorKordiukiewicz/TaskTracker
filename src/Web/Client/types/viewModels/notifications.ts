import type { NotificationContext } from "../enums";

export interface NotificationsVM {
    notifications: NotificationVM[];
}

export interface NotificationVM {
    id: string;
    message: string;
    occurredAt: Date;
    context: NotificationContext;
    contextEntityId: string;
    contextEntityName: string;
    taskShortId?: number;
}