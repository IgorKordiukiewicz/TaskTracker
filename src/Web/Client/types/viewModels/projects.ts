import type { ProjectPermissions } from "../enums";
import type { NavDataVM } from "./shared";

export interface ProjectsVM {
    projects: ProjectVM[];
}

export interface ProjectVM {
    id: string;
    name: string;
}

export interface ProjectNavDataVM {
    project: NavDataVM;
    organization: NavDataVM;
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

export interface ProjectRolesVM {
    roles: ProjectRoleVM[];
}

export interface ProjectRoleVM {
    id: string;
    name: string;
    permissions: ProjectPermissions;
    modifiable: boolean;
}