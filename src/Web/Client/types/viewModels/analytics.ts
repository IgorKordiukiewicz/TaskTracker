import { allTaskPriorities } from './../enums';
import type { TaskPriority } from "../enums";

export interface TaskAnalyticsVM {
    dates: Date[];

    countByStatusId: Record<string, number>;
    countByPriority: Record<string, number>; // has to be string

    dailyCountByStatusId: Record<string, number[]>;
    dailyCountByPriority: Record<string, number[]>; // has to be string
}