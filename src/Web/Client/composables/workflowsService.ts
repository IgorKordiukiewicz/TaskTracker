import type { AddWorkflowStatusDto, AddWorkflowTransitionDto } from "~/types/dtos/workflows";
import type { WorkflowVM } from "~/types/viewModels/workflows";

export const useWorkflowsService = () => {
    const api = useApi();

    return {
        async getWorkflow(projectId: string) {
            return await api.sendGetRequest<WorkflowVM>(`workflows?projectId=${projectId}`);
        },
        async addStatus(id: string, projectId: string, model: AddWorkflowStatusDto) {
            await api.sendPostRequest(`workflows/${id}/statuses`, model, { 'ProjectId': projectId });
        },
        async addTransition(id: string, projectId: string, model: AddWorkflowTransitionDto) {
            await api.sendPostRequest(`workflows/${id}/transitions`, model, { 'ProjectId': projectId });
        }
    }
}