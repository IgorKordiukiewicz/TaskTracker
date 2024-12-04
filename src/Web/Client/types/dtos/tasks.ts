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

export class AddTaskCommentDto {
    content: string = '';
}

export class AddTaskLoggedTimeDto {
    minutes: number = 0;
    day: Date = new Date();
}

export class UpdateTaskEstimatedTimeDto {
    minutes: number = 0;
}

export class UpdateTaskTitleDto {
    title: string = '';
}

export class UpdateTaskBoardDto {
    projectId: string = '';
    columns: UpdateTaskBoardColumnDto[] = [];
}

export class UpdateTaskBoardColumnDto {
    statusId: string = '';
    tasksIds: string[] = [];

    constructor(statusId: string, tasksIds: string[]) {
        this.statusId = statusId;
        this.tasksIds = tasksIds;
    }
}