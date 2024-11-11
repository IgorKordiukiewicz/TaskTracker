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
    roleId: string;
}