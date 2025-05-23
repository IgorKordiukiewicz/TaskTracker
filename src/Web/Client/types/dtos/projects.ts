export class CreateProjectDto {
    name: string = '';
}

export class CreateProjectInvitationDto {
    userId: string = '';
    expirationDays?: number;
}

export class UpdateProjectNameDto {
    name: string = '';
}