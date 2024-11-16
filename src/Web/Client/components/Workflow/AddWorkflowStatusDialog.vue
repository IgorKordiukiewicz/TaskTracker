<template>
    <ActionDialog header="Add a new status" submit-label="Add" ref="dialog" @submit="addStatus" :submit-disabled="submitDisabled">
        <LabeledInput label="Name">
            <InputText v-model="name" autocomplete="off" class="w-full" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { AddWorkflowStatusDto } from '~/types/dtos/workflows';

defineExpose({ show });
const emit = defineEmits([ 'onAdd' ]);
const props = defineProps({
    workflowId: { type: String, required: true },
    projectId: { type: String, required: true },
})

const workflowsService = useWorkflowsService();

const dialog = ref();
const name = ref();

const submitDisabled = computed(() => {
    return !name.value;
})

function show() {
    dialog.value.show();
}

async function addStatus() {
    const model = new AddWorkflowStatusDto();
    model.name = name.value;
    await workflowsService.addStatus(props.workflowId, props.projectId, model);
    emit('onAdd', model.name);
}
</script>