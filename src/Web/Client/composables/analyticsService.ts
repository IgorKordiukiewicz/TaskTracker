import type { TotalTaskPrioritiesByDayVM, TotalTaskPrioritiesVM, TotalTaskStatusesByDayVM, TotalTaskStatusesVM } from "~/types/viewModels/analytics";

export const useAnalyticsService = () => {
    const api = useApi();

    return {
        async getTotalTaskStatuses(projectId: string) {
            return await api.sendGetRequest<TotalTaskStatusesVM>(`analytics/${projectId}/tasks/statuses`);
        },
        async getTotalTaskStatusesByDay(projectId: string) {
            return await api.sendGetRequest<TotalTaskStatusesByDayVM>(`analytics/${projectId}/tasks/daily-statuses`);
        },
        async getTotalTaskPriorities(projectId: string) {
            return await api.sendGetRequest<TotalTaskPrioritiesVM>(`analytics/${projectId}/tasks/priorities`);
        },
        async getTotalTaskPrioritiesByDay(projectId: string) {
            return await api.sendGetRequest<TotalTaskPrioritiesByDayVM>(`analytics/${projectId}/tasks/daily-priorities`);
        }
    }
}