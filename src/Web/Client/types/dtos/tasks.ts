export class CreateTaskDto {
    title: string = '';
    description: string = '';
    priority: number = 1;
    assigneeMemberId?: string;
}

export class UpdateTaskDescriptionDto {
    description: string = '';
}