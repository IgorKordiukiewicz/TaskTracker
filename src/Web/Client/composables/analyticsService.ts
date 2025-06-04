import type { TaskAnalyticsVM } from "~/types/viewModels/analytics";

export const useAnalyticsService = () => {
    const api = useApi();

    return {
        async getTaskAnalytics(projectId: string) {
            return await api.sendGetRequest<TaskAnalyticsVM>(`analytics/${projectId}/tasks`);
        }
    }
}