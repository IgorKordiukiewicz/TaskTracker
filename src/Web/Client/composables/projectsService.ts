import type { CreateProjectDto } from "~/types/dtos/projects";
import type { UpdateMemberRoleDto } from "~/types/dtos/shared";
import type { ProjectRolesVM, ProjectMembersVM, ProjectNavDataVM, ProjectsVM } from "~/types/viewModels/projects";

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
        }
    }
}