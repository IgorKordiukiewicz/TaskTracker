import type { WorkflowVM } from "~/types/viewModels/workflows";

export const useWorkflowsService = () => {
    const api = useApi();

    return {
        async getWorkflow(projectId: string) {
            return await api.sendGetRequest<WorkflowVM>(`workflows?projectId=${projectId}`);
        }
    }
}