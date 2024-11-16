import type { AddProjectMemberDto, CreateProjectDto, UpdateProjectNameDto } from "~/types/dtos/projects";
import type { CreateRoleDto, DeleteRoleDto, RemoveMemberDto, UpdateMemberRoleDto, UpdateRoleNameDto, UpdateRolePermissionsDto } from "~/types/dtos/shared";
import type { ProjectRolesVM, ProjectMembersVM, ProjectNavDataVM, ProjectsVM, ProjectSettingsVM, UserProjectPermissionsVM } from "~/types/viewModels/projects";

export const useProjectsService = () => {
    const api = useApi();

    return {
        async getProjects(organizationId: string) {
            return await api.sendGetRequest<ProjectsVM>(`projects?organizationId=${organizationId}`);
        },
        async getNavData(id: string) {
            return await api.sendGetRequest<ProjectNavDataVM>(`projects/${id}/nav-data`);
        },
        async createProject(model: CreateProjectDto) {
            await api.sendPostRequest('projects', model, { 'OrganizationId': model.organizationId });
        },
        async getMembers(id: string) {
            return await api.sendGetRequest<ProjectMembersVM>(`projects/${id}/members`);
        },
        async getRoles(id: string) {
            return await api.sendGetRequest<ProjectRolesVM>(`projects/${id}/roles`);
        },
        async updateMemberRole(id: string, model: UpdateMemberRoleDto) {
            await api.sendPostRequest(`projects/${id}/members/role`, model);
        },
        async createRole(id: string, model: CreateRoleDto) {
            await api.sendPostRequest(`projects/${id}/roles`, model);
        },
        async updateRolePermissions(id: string, model: UpdateRolePermissionsDto) {
            await api.sendPostRequest(`projects/${id}/roles/permissions`, model);
        },
        async updateRoleName(id: string, model: UpdateRoleNameDto) {
            await api.sendPostRequest(`projects/${id}/roles/name`, model);
        },
        async deleteRole(id: string, model: DeleteRoleDto) {
            await api.sendPostRequest(`projects/${id}/roles/delete`, model);
        },
        async addMember(id: string, model: AddProjectMemberDto) {
            await api.sendPostRequest(`projects/${id}/members`, model);
        },
        async removeMember(id: string, model: RemoveMemberDto) {
            await api.sendPostRequest(`projects/${id}/members/remove`, model);
        },
        async getSettings(id: string) {
            return await api.sendGetRequest<ProjectSettingsVM>(`projects/${id}/settings`);
        },
        async updateName(id: string, model: UpdateProjectNameDto) {
            await api.sendPostRequest(`projects/${id}/name`, model);
        },
        async deleteProject(id: string) {
            await api.sendPostRequest(`projects/${id}/delete`, undefined);
        },
        async getUserPermissions(id: string) {
            return await api.sendGetRequest<UserProjectPermissionsVM>(`projects/${id}/permissions`);
        }
    }
}