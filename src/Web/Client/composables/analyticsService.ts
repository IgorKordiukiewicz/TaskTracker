import type { TotalTaskStatusesVM } from "~/types/viewModels/analytics";

export const useAnalyticsService = () => {
    const api = useApi();

    return {
        async getTotalTaskStatuses(projectId: string) {
            return await api.sendGetRequest<TotalTaskStatusesVM>(`analytics/${projectId}/tasks/statuses`);
        }
    }
}