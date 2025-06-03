export interface TotalTaskStatusesVM {
    countByStatusId: Record<string, number>;
}

export interface TotalTaskStatusesByDayVM {
    dates: Date[];
    countsByStatusId: Record<string, number[]>;
}