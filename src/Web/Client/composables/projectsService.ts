import type { CreateProjectDto, CreateProjectInvitationDto, UpdateProjectNameDto } from "~/types/dtos/projects";
import type { CreateRoleDto, DeleteRoleDto, RemoveMemberDto, UpdateMemberRoleDto, UpdateRoleNameDto, UpdateRolePermissionsDto } from "~/types/dtos/projects";
import type { RolesVM, ProjectMembersVM, ProjectInfoVM, ProjectsVM, ProjectSettingsVM, UserProjectPermissionsVM, ProjectInvitationsVM, UserProjectInvitationsVM } from "~/types/viewModels/projects";

export const useProjectsService = () => {
    const api = useApi();

    return {
        async getProjects() {
            return await api.sendGetRequest<ProjectsVM>(`projects`);
        },
        async getInfo(id: string) {
            return await api.sendGetRequest<ProjectInfoVM>(`projects/${id}`);
        },
        async createProject(model: CreateProjectDto) {
            await api.sendPostRequest('projects', model);
        },
        async createInvitation(id: string, model: CreateProjectInvitationDto) {
            await api.sendPostRequest(`projects/${id}/invitations`, model);
        },
        async getInvitations(id: string) {
            return await api.sendGetRequest<ProjectInvitationsVM>(`projects/${id}/invitations`);
        },
        async getUserInvitations() {
            return await api.sendGetRequest<UserProjectInvitationsVM>('projects/invitations');
        },
        async acceptInvitation(invitationId: string) {
            await api.sendPostRequest(`projects/invitations/${invitationId}/accept`, undefined);
        },
        async declineInvitation(invitationId: string) {
            await api.sendPostRequest(`projects/invitations/${invitationId}/decline`, undefined);
        },
        async cancelInvitation(id: string, invitationId: string) {
            await api.sendPostRequest(`projects/invitations/${invitationId}/cancel`, undefined,  { 'ProjectId': id });
        },
        async getMembers(id: string) {
            return await api.sendGetRequest<ProjectMembersVM>(`projects/${id}/members`);
        },
        async getRoles(id: string) {
            return await api.sendGetRequest<RolesVM>(`projects/${id}/roles`);
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
        },
        async leaveProject(id: string) {
            await api.sendPostRequest(`projects/${id}/leave`, undefined);
        },
    }
}