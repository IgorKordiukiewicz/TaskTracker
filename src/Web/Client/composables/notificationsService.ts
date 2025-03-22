import type { NotificationsVM } from "~/types/viewModels/notifications";

export const useNotificationsService = () => {
    const api = useApi();

    return {
        async getNotifications() {
            return await api.sendGetRequest<NotificationsVM>('notifications');
        },
        async getUnreadCount() {
            return await api.sendGetRequest<number>('notifications/unread-count');
        },
        async read(id: string) {
            await api.sendPostRequest(`notifications/${id}/read`, undefined);
        }
    }
}