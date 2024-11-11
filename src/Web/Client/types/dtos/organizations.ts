import type { OrganizationPermissions } from "../enums";

export class CreateOrganizationDto {
    name: string = '';
}

export class CreateOrganizationInvitationDto {
    userId: string = '';
}

export class UpdateOrganizationMemberRoleDto {
    memberId: string = '';
    roleId: string = '';
}

export class CreateOrganizationRoleDto {
    name: string = '';
    permissions: OrganizationPermissions = 0;
}

export class UpdateOrganizationRolePermissionsDto {
    roleId: string = '';
    permissions: OrganizationPermissions = 0;
}

export class UpdateOrganizationRoleNameDto {
    roleId: string = '';
    name: string = '';
}