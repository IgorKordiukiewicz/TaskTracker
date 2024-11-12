export interface NavDataVM {
    id: string;
    name: string;
}

export interface MemberVM {
    id: string;
    userId: string;
    name: string;
    email: string;
    roleId: string;
    roleName: string;
}

export interface RoleVM {
    id: string;
    name: string;
    modifiable: boolean;
}