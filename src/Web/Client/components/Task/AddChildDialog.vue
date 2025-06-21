<template>
    <ActionDialog header="Add child" submit-label="Confirm" @submit="submit" ref="dialog" :submit-disabled="submitDisabled" v-if="availableChildren">
        <Select v-model="selectedChild" :options="availableChildren.tasks" filter option-label="title" placeholder="Select child" class="w-full">
            <template #value="{ value }">

            </template>
            <template #option="{ option }">
                <span>
                    [#{{option.shortId}}] {{ option.title }}
                </span>
            </template>
        </Select>
    </ActionDialog>
</template>

<script setup lang="ts">
import { AddTaskRelationDto } from '~/types/dtos/tasks';

defineExpose({ show });
const emit = defineEmits([ 'onSubmit' ]);
const props = defineProps({
    taskId: { type: String, required: true },
    projectId: { type: String, required: true }
})

const tasksService = useTasksService();

const dialog = ref();
const availableChildren = ref();
const selectedChild = ref();

const submitDisabled = computed(() => {
    return !selectedChild.value;
})

async function show() {
    await updateAvailableChildren();
    dialog.value.show();
}

async function submit() {
    const model = new AddTaskRelationDto();
    model.parentId = props.taskId;
    model.childId = selectedChild.value.id;
    emit('onSubmit', model);

    selectedChild.value = undefined;
}

async function updateAvailableChildren() {
    availableChildren.value = await tasksService.getAvailableChildren(props.taskId, props.projectId);
}
</script>