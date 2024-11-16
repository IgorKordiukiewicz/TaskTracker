import type { TaskStatusDeletionEligibility } from "../enums";

export interface WorkflowVM {
    id: string;
    statuses: WorkflowStatusVM[];
    transitions: WorkflowTransitionVM[];
}

export interface WorkflowStatusVM {
    id: string;
    name: string;
    initial: boolean;
    deletionEligibility: TaskStatusDeletionEligibility;
}

export interface WorkflowTransitionVM {
    fromStatusId: string;
    toStatusId: string;
}