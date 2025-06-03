import type { TotalTaskStatusesByDayVM, TotalTaskStatusesVM } from "~/types/viewModels/analytics";

export const useAnalyticsService = () => {
    const api = useApi();

    return {
        async getTotalTaskStatuses(projectId: string) {
            return await api.sendGetRequest<TotalTaskStatusesVM>(`analytics/${projectId}/tasks/statuses`);
        },
        async getTotalTaskStatusesByDay(projectId: string) {
            return await api.sendGetRequest<TotalTaskStatusesByDayVM>(`analytics/${projectId}/tasks/daily-statuses`);
        }
    }
}