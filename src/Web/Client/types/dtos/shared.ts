export class UpdateMemberRoleDto {
    memberId: string = '';
    roleId: string = '';
}

export class CreateRoleDto {
    name: string = '';
    permissions: number = 0;
}

export class UpdateRolePermissionsDto {
    roleId: string = '';
    permissions: number = 0;
}

export class UpdateRoleNameDto {
    roleId: string = '';
    name: string = '';
}

export class DeleteRoleDto {
    roleId: string = '';
}

export class RemoveMemberDto {
    memberId: string = '';
}