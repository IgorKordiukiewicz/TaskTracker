import type { OrganizationPermissions } from "../enums";
import type { NavDataVM } from "./shared";

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

export interface OrganizationMemberVM {
    id: string;
    userId: string;
    name: string;
    email: string;
    roleId: string;
    roleName: string;
    owner: boolean;
}

export interface OrganizationRolesVM {
    roles: OrganizationRoleVM[];
}

export interface OrganizationRoleVM {
    id: string;
    name: string;
    permissions: OrganizationPermissions;
    modifiable: boolean;
}