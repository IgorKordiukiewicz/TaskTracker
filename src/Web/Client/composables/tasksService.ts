import type { AddTaskLoggedTimeDto, AddTaskCommentDto, CreateTaskDto, UpdateTaskEstimatedTimeDto, UpdateTaskAssigneeDto, UpdateTaskDescriptionDto, UpdateTaskPriorityDto, UpdateTaskStatusDto, UpdateTaskTitleDto, UpdateTaskBoardDto } from "~/types/dtos/tasks";
import type { TaskRelationshipsVM, TaskActivitiesVM, TaskCommentsVM, TasksVM } from "~/types/viewModels/tasks";

export const useTasksService = () => {
    const api = useApi();

    return {
        async getTasks(projectId: string) {
            return await api.sendGetRequest<TasksVM>('tasks', { 'ProjectId': projectId });
        },
        async createTask(projectId: string, model: CreateTaskDto) {
            await api.sendPostRequest('tasks', model, { 'ProjectId': projectId });
        },
        async getTask(shortId: number, projectId: string) {
            return (await api.sendGetRequest<TasksVM>(`tasks/${shortId}`, { 'ProjectId': projectId }))?.tasks[0];
        },
        async updateDescription(id: string, projectId: string, model: UpdateTaskDescriptionDto) {
            await api.sendPostRequest(`tasks/${id}/description`, model, { 'ProjectId': projectId });
        },
        async updatePriority(id: string, projectId: string, model: UpdateTaskPriorityDto) {
            await api.sendPostRequest(`tasks/${id}/priority`, model, { 'ProjectId': projectId });
        },
        async updateAssignee(id: string, projectId: string, model: UpdateTaskAssigneeDto) {
            await api.sendPostRequest(`tasks/${id}/assignee`, model, { 'ProjectId': projectId });
        },
        async updateStatus(id: string, projectId: string, model: UpdateTaskStatusDto) {
            await api.sendPostRequest(`tasks/${id}/status`, model, { 'ProjectId': projectId });
        },
        async addComment(id: string, projectId: string, model: AddTaskCommentDto) {
            await api.sendPostRequest(`tasks/${id}/comments`, model, { 'ProjectId': projectId });
        },
        async addLoggedTime(id: string, projectId: string, model: AddTaskLoggedTimeDto) {
            await api.sendPostRequest(`tasks/${id}/logged-time`, model, { 'ProjectId': projectId });
        },
        async updateEstimatedTime(id: string, projectId: string, model: UpdateTaskEstimatedTimeDto) {
            await api.sendPostRequest(`tasks/${id}/estimated-time`, model, { 'ProjectId': projectId });
        },
        async getComments(id: string, projectId: string) {
            return await api.sendGetRequest<TaskCommentsVM>(`tasks/${id}/comments`, { 'ProjectId': projectId });
        },
        async getActivities(id: string, projectId: string) {
            return await api.sendGetRequest<TaskActivitiesVM>(`tasks/${id}/activities`, { 'ProjectId': projectId });
        },
        async updateTitle(id: string, projectId: string, model: UpdateTaskTitleDto) {
            await api.sendPostRequest(`tasks/${id}/title`, model, { 'ProjectId': projectId });
        },
        async deleteTask(id: string, projectId: string) {
            await api.sendPostRequest(`tasks/${id}/delete`, undefined, { 'ProjectId': projectId });
        },
        async updateBoard(projectId: string, model: UpdateTaskBoardDto) {
            await api.sendPostRequest('tasks/update-board', model, { 'ProjectId': projectId });
        },
        async getRelationships(id: string, projectId: string) {
            return await api.sendGetRequest<TaskRelationshipsVM>(`tasks/${id}/relationships`, { 'ProjectId': projectId });
        }
    }
}