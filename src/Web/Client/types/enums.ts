export enum OrganizationPermissions {
    None = 0,
    CreateProjects = 1 << 0, // 1
    InviteMembers = 1 << 1, // 2
    RemoveMembers = 1 << 2, // 4
    ManageRoles = 1 << 3, // 8
}

export enum OrganizationInvitationState
{
    Pending,
    Accepted,
    Declined,
    Canceled
}