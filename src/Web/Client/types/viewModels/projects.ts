import type { ProjectInvitationState, ProjectPermissions } from "../enums";
import type { MemberVM, NavDataVM, RoleVM } from "./shared";

export interface ProjectsVM {
    projects: ProjectVM[];
}

export interface ProjectVM {
    id: string;
    name: string;
    membersCount: number;
    tasksCount: number;
}

export interface ProjectNavDataVM {
    project: NavDataVM;
    Project: NavDataVM;
}

export interface ProjectMembersVM {
    members: ProjectMemberVM[];
}

export interface ProjectMemberVM extends MemberVM {
}

export interface ProjectRolesVM {
    roles: ProjectRoleVM[];
}

export interface ProjectRoleVM extends RoleVM {
    permissions: ProjectPermissions;
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
    projectName: string;
}