import type { ProjectInvitationState, ProjectPermissions } from "../enums";

export interface ProjectsVM {
    projects: ProjectVM[];
}

export interface ProjectVM {
    id: string;
    name: string;
    membersCount: number;
    tasksCount: number;
}

export interface ProjectMembersVM {
    members: ProjectMemberVM[];
}

export interface ProjectMemberVM {
    id: string;
    userId: string;
    name: string;
    email: string;
    roleId: string;
    roleName: string;
}

export interface RolesVM {
    roles: RoleVM[];
}

export interface RoleVM {
    permissions: ProjectPermissions;
    id: string;
    name: string;
    modifiable: boolean;
    owner: boolean;
}

export interface ProjectSettingsVM {
    name: string;
}

export interface UserProjectPermissionsVM {
    permissions: ProjectPermissions;
    isOwner: boolean;
}

export interface ProjectInvitationsVM {
    invitations: ProjectInvitationVM[];
    totalPagesCount: number;
}

export interface ProjectInvitationVM {
    id: string;
    userEmail: string;
    state: ProjectInvitationState;
    createdAt: Date;
    finalizedAt: Date;
    expirationDate?: Date;
}

export interface UserProjectInvitationsVM {
    invitations: UserProjectInvitationVM[];
}

export interface UserProjectInvitationVM {
    id: string;
    projectId: string;
    projectName: string;
}

export interface ProjectInfoVM {
    id: string;
    name: string;
}