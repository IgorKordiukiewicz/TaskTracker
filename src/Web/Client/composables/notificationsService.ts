import type { NotificationsVM } from "~/types/viewModels/notifications";

export const useNotificationsService = () => {
    const api = useApi();

    return {
        async getNotifications() {
            return await api.sendGetRequest<NotificationsVM>('notifications');
        }
    }
}