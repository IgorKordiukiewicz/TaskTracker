<template>
    <ActionDialog header="Create a task" submit-label="Create" @submit="createTask" ref="dialog">
        <div class="flex flex-col gap-2">
            <div class="flex flex-col gap-1">
                <label for="title">Title</label>
                <InputText id="title" v-model="model.title" autocomplete="off" class="w-full" />
            </div>
            <div class="flex flex-col gap-1">
                <label for="description">Description</label>
                <InputText id="description" v-model="model.description" autocomplete="off" class="w-full" />
            </div>
            <div class="flex flex-col gap-1">
                <label for="priority">Priority</label>
                <Select v-model="selectedPriority" :options="priorities" option-label="name" class="w-full" />
            </div>
            <div class="flex flex-col gap-1">
                <label for="assignee">Assignee</label>
                <Select v-model="model.assigneeMemberId" :options="props.members.members" option-label="name" option-value="id" class="w-full" showClear />
            </div>
        </div>
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