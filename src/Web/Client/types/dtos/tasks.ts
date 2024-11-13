import type { TaskPriority } from "../enums";

export class CreateTaskDto {
    title: string = '';
    description: string = '';
    priority: number = 1;
    assigneeMemberId?: string;
}

export class UpdateTaskDescriptionDto {
    description: string = '';
}

export class UpdateTaskPriorityDto {
    priority: TaskPriority = 0;
}

export class UpdateTaskAssigneeDto {
    memberId?: string;
}

export class UpdateTaskStatusDto {
    statusId: string = '';
}