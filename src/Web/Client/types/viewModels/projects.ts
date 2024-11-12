import type { ProjectPermissions } from "../enums";
import type { MemberVM, NavDataVM, RoleVM } from "./shared";

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

export interface ProjectMemberVM extends MemberVM {
}

export interface ProjectRolesVM {
    roles: ProjectRoleVM[];
}

export interface ProjectRoleVM extends RoleVM {
    permissions: ProjectPermissions;
}