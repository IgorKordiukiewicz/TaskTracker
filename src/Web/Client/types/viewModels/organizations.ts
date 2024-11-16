import type { OrganizationInvitationState, OrganizationPermissions } from "../enums";
import type { MemberVM, NavDataVM, RoleVM } from "./shared";

export interface OrganizationsVM
{
    organizations: OrganizationVM[];
}

export interface OrganizationVM
{
    id: string;
    name: string;
}

export interface OrganizationNavDataVM {
    organization: NavDataVM;
}

export interface OrganizationMembersVM {
    members: OrganizationMemberVM[];
}

export interface OrganizationMemberVM extends MemberVM {
    owner: boolean;
}

export interface OrganizationRolesVM {
    roles: OrganizationRoleVM[];
}

export interface OrganizationRoleVM extends RoleVM {
    permissions: OrganizationPermissions;
}

export interface OrganizationInvitationsVM {
    invitations: OrganizationInvitationVM[];
    totalPagesCount: number;
}

export interface OrganizationInvitationVM {
    id: string;
    userEmail: string;
    state: OrganizationInvitationState;
    createdAt: Date;
    finalizedAt: Date;
}

export interface UserOrganizationInvitationsVM {
    invitations: UserOrganizationInvitationVM[];
}

export interface UserOrganizationInvitationVM {
    id: string;
    organizationName: string;
}

export interface OrganizationSettingsVM {
    name: string;
    ownerId: string;
}