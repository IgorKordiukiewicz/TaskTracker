import type { ProjectsVM } from "~/types/viewModels/projects";

export const useProjectsService = () => {
    const api = useApi();

    return {
        async getProjects(organizationId: string) {
            return await api.sendGetRequest<ProjectsVM>(`projects?organizationId=${organizationId}`);
        }
    }
}