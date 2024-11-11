import type { CreateTaskDto } from "~/types/dtos/tasks";
import type { TasksVM } from "~/types/viewModels/tasks";

export const useTasksService = () => {
    const api = useApi();

    return {
        async getTasks(projectId: string) {
            return await api.sendGetRequest<TasksVM>('tasks', { 'ProjectId': projectId });
        },
        async createTask(projectId: string, model: CreateTaskDto) {
            await api.sendPostRequest('tasks', model, { 'ProjectId': projectId });
        }
    }
}