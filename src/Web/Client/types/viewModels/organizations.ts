import type { NavDataVM } from "./shared";

export interface OrganizationsVM
{
    organizations: OrganizationVM[];
}

export interface OrganizationVM
{
    id: string;
    name: string;
}

export interface OrganizationNavDataVM {
    organization: NavDataVM;
}