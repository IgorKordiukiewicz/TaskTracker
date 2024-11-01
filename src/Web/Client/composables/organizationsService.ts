import type { OrganizationNavDataVM, OrganizationsVM } from "~/types/viewModels/organizations";

export const useOrganizationsService = () => {
    const api = useApi();

    return {
        async getOrganizations() {
            return await api.sendGetRequest<OrganizationsVM>('organizations');
        },
        async getNavData(id: string) {
            return await api.sendGetRequest<OrganizationNavDataVM>(`organizations/${id}/nav-data`);
        }
    }
}