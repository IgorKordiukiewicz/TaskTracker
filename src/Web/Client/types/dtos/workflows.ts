export class AddWorkflowStatusDto {
    name: string = '';
}

export class AddWorkflowTransitionDto {
    fromStatusId: string = '';
    toStatusId: string = '';
}