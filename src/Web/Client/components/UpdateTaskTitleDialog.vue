<template>
    <ActionDialog header="Update title" submit-label="Confirm" @submit="updateTitle" ref="dialog">
        <LabeledInput label="Title">
            <InputText v-model="model.title" autocomplete="off" class="w-full" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { UpdateTaskTitleDto } from '~/types/dtos/tasks';

defineExpose({ show });
const emit = defineEmits([ 'onUpdate' ]);
const props = defineProps({
    id: { type: String, required: true },
    projectId: { type: String, required: true }
})

const tasksService = useTasksService();

const dialog = ref();
const model = ref(new UpdateTaskTitleDto());

function show(title: string) {
    model.value.title = title;
    dialog.value.show();
}

async function updateTitle() {
    await tasksService.updateTitle(props.id, props.projectId, model.value);
    emit('onUpdate');
}

</script>