import type { ProjectNavDataVM, ProjectsVM } from "~/types/viewModels/projects";

export const useProjectsService = () => {
    const api = useApi();

    return {
        async getProjects(organizationId: string) {
            return await api.sendGetRequest<ProjectsVM>(`projects?organizationId=${organizationId}`);
        },
        async getNavData(id: string) {
            return await api.sendGetRequest<ProjectNavDataVM>(`projects/${id}/nav-data`);
        }
    }
}