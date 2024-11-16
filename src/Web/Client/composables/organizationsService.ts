import type { CreateOrganizationDto, CreateOrganizationInvitationDto, UpdateOrganizationNameDto } from "~/types/dtos/organizations";
import type { CreateRoleDto, DeleteRoleDto, RemoveMemberDto, UpdateMemberRoleDto, UpdateRoleNameDto, UpdateRolePermissionsDto } from '~/types/dtos/shared';
import type { OrganizationRolesVM, OrganizationMembersVM, OrganizationNavDataVM, OrganizationsVM, OrganizationInvitationsVM, 
    UserOrganizationInvitationsVM, OrganizationSettingsVM, UserOrganizationPermissionsVM } from "~/types/viewModels/organizations";

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
        async updateMemberRole(id: string, model: UpdateMemberRoleDto) {
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
        async cancelInvitation(id: string, invitationId: string) {
            await api.sendPostRequest(`organizations/invitations/${invitationId}/cancel`, '',  { 'OrganizationId': id });
        },
        async createRole(id: string, model: CreateRoleDto) {
            await api.sendPostRequest(`organizations/${id}/roles`, model);
        },
        async updateRolePermissions(id: string, model: UpdateRolePermissionsDto) {
            await api.sendPostRequest(`organizations/${id}/roles/permissions`, model);
        },
        async updateRoleName(id: string, model: UpdateRoleNameDto) {
            await api.sendPostRequest(`organizations/${id}/roles/name`, model);
        },
        async deleteRole(id: string, model: DeleteRoleDto) {
            await api.sendPostRequest(`organizations/${id}/roles/delete`, model);
        },
        async removeMember(id: string, model: RemoveMemberDto) {
            await api.sendPostRequest(`organizations/${id}/members/remove`, model);
        },
        async getSettings(id: string) {
            return await api.sendGetRequest<OrganizationSettingsVM>(`organizations/${id}/settings`);
        },
        async updateName(id: string, model: UpdateOrganizationNameDto) {
            await api.sendPostRequest(`organizations/${id}/name`, model);
        },
        async deleteOrganization(id: string) {
            await api.sendPostRequest(`organizations/${id}/delete`, '');
        },
        async getUserPermissions(id: string) {
            return await api.sendGetRequest<UserOrganizationPermissionsVM>(`organizations/${id}/permissions`);
        }
    }
}