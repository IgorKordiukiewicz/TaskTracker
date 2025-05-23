export interface NotificationsVM {
    notifications: NotificationVM[];
}

export interface NotificationVM {
    id: string;
    message: string;
    occurredAt: Date;
    contextEntityId: string;
    contextEntityName: string;
    taskShortId?: number;
}