import type { TaskPriority } from "../enums";

export interface TotalTaskStatusesVM {
    countByStatusId: Record<string, number>;
}

export interface TotalTaskStatusesByDayVM {
    dates: Date[];
    countsByStatusId: Record<string, number[]>;
}

export interface TotalTaskPrioritiesVM {
    countByPriority: Record<TaskPriority, number>;
}

export interface TotalTaskPrioritiesByDayVM {
    dates: Date[];
    countsByPriority: Record<TaskPriority, number[]>;
}