import { UpdateOrganizationMemberRoleDto } from './../types/dtos/organizations';
import type { CreateOrganizationDto, CreateOrganizationInvitationDto, CreateOrganizationRoleDto, UpdateOrganizationRoleNameDto, UpdateOrganizationRolePermissionsDto } from "~/types/dtos/organizations";
import type { OrganizationRolesVM, OrganizationMembersVM, OrganizationNavDataVM, OrganizationsVM, OrganizationInvitationsVM, UserOrganizationInvitationsVM } from "~/types/viewModels/organizations";

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
        },
        async getUserInvitations() {
            return await api.sendGetRequest<UserOrganizationInvitationsVM>('organizations/invitations');
        },
        async acceptInvitation(invitationId: string) {
            await api.sendPostRequest(`organizations/invitations/${invitationId}/accept`, '');
        },
        async declineInvitation(invitationId: string) {
            await api.sendPostRequest(`organizations/invitations/${invitationId}/decline`, '');
        },
        async createRole(id: string, model: CreateOrganizationRoleDto) {
            await api.sendPostRequest(`organizations/${id}/roles`, model);
        },
        async updateRolePermissions(id: string, model: UpdateOrganizationRolePermissionsDto) {
            await api.sendPostRequest(`organizations/${id}/roles/permissions`, model);
        },
        async updateRoleName(id: string, model: UpdateOrganizationRoleNameDto) {
            await api.sendPostRequest(`organizations/${id}/roles/name`, model);
        }
    }
}