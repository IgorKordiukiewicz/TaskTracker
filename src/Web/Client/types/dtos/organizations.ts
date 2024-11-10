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