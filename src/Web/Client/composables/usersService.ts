import type { UsersVM } from "~/types/viewModels/users";

export const useUsersService = () => {
    const api = useApi();

    return {
        async getAvailableForInvitation(organizationId: string, search: string) {
            return await api.sendGetRequest<UsersVM>(`users/available-for-invitation?organizationId=${organizationId}&searchValue=${search}`);
        },
        async getAvailableForProject(projectId: string) {
            return await api.sendGetRequest<UsersVM>(`users/available-for-project?projectId=${projectId}`);
        }
    }
}