<template>
    <ActionDialog header="Create a task" submit-label="Create" @submit="createTask" ref="dialog">
        <LabeledInput label="Title">
            <InputText v-model="model.title" autocomplete="off" class="w-full" />
        </LabeledInput>
        <LabeledInput label="Description">
            <InputText id="description" v-model="model.description" autocomplete="off" class="w-full" />
        </LabeledInput>
        <LabeledInput label="Priority">
            <Select v-model="selectedPriority" :options="priorities" option-label="name" class="w-full" />
        </LabeledInput>
        <LabeledInput label="Assignee">
            <Select v-model="model.assigneeMemberId" :options="props.members.members" option-label="name" option-value="id" class="w-full" showClear />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { CreateTaskDto } from '~/types/dtos/tasks';
import { TaskPriority } from '~/types/enums';
import type { ProjectMembersVM } from '~/types/viewModels/projects';

defineExpose({ show });
const emit = defineEmits([ 'onCreate' ]);
const props = defineProps({
    projectId: { type: String, required: true },
    members: { type: Object as PropType<ProjectMembersVM>, required: true },
});

const tasksService = useTasksService();

const dialog = ref();
const model = ref(new CreateTaskDto());

const priorities = ref([
    { key: TaskPriority.Low, name: TaskPriority[TaskPriority.Low] },
    { key: TaskPriority.Normal, name: TaskPriority[TaskPriority.Normal] },
    { key: TaskPriority.High, name: TaskPriority[TaskPriority.High] },
    { key: TaskPriority.Urgent, name: TaskPriority[TaskPriority.Urgent] },
]);
const selectedPriority = ref({ key: TaskPriority.Normal, name: TaskPriority[TaskPriority.Normal] });

function show() {
    dialog.value.show();
}

async function createTask() {
    model.value.priority = selectedPriority.value.key;
    await tasksService.createTask(props.projectId, model.value);

    model.value = new CreateTaskDto();
    selectedPriority.value = { key: TaskPriority.Normal, name: TaskPriority[TaskPriority.Normal] }; // TODO: Refactor priority select

    emit('onCreate');
}
</script>