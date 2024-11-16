export class CreateProjectDto {
    organizationId: string = '';
    name: string = '';
}

export class AddProjectMemberDto {
    userId: string = '';
}

export class UpdateProjectNameDto {
    name: string = '';
}