export class CreateOrganizationDto {
    name: string = '';
}

export class CreateOrganizationInvitationDto {
    userId: string = '';
    expirationDays?: number;
}

export class UpdateOrganizationNameDto {
    name: string = '';
}