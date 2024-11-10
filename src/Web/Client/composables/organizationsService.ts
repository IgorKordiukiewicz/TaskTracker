import { UpdateOrganizationMemberRoleDto } from './../types/dtos/organizations';
import type { CreateOrganizationDto, CreateOrganizationInvitationDto } from "~/types/dtos/organizations";
import type { OrganizationRolesVM, OrganizationMembersVM, OrganizationNavDataVM, OrganizationsVM, OrganizationInvitationsVM } from "~/types/viewModels/organizations";

export const useOrganizationsService = () => {
    const api = useApi();

    return {
        async getOrganizations() {
            return await api.sendGetRequest<OrganizationsVM>('organizations');
        },
        async getNavData(id: string) {
            return await api.sendGetRequest<OrganizationNavDataVM>(`organizations/${id}/nav-data`);
        },
        async createOrganization(model: CreateOrganizationDto) {
            await api.sendPostRequest('organizations', model);
        },
        async getMembers(id: string) {
            return await api.sendGetRequest<OrganizationMembersVM>(`organizations/${id}/members`);
        },
        async getRoles(id: string) {
            return await api.sendGetRequest<OrganizationRolesVM>(`organizations/${id}/roles`);
        },
        async createInvitation(id: string, model: CreateOrganizationInvitationDto) {
            await api.sendPostRequest(`organizations/${id}/invitations`, model);
        },
        async getInvitations(id: string) {
            return await api.sendGetRequest<OrganizationInvitationsVM>(`organizations/${id}/invitations`);
        },
        async updateMemberRole(id: string, model: UpdateOrganizationMemberRoleDto) {
            await api.sendPostRequest(`organizations/${id}/members/role`, model);
        }
    }
}