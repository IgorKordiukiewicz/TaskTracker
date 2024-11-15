<template>
    <ActionDialog header="Add a new transition" submit-label="Add" ref="dialog" @submit="addTransition">
        <LabeledInput label="From Status">
            <Select v-model="selectedFromStatus" :options="availableFromStatuses" option-label="name" option-value="id" />
        </LabeledInput>
        <LabeledInput label="To Status">
            <Select v-model="selectedToStatus" :options="availableToStatuses" option-label="name" option-value="id" :disabled="!selectedFromStatus" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import { AddWorkflowTransitionDto } from '~/types/dtos/workflows';
import type { WorkflowStatusVM, WorkflowVM } from '~/types/viewModels/workflows';


defineExpose({ show });
const emit = defineEmits([ 'onAdd' ]);
const props = defineProps({
    projectId: { type: String, required: true },
    workflow: { type: Object as PropType<WorkflowVM>, required: true }
})

const workflowsService = useWorkflowsService();

const dialog = ref();
const selectedFromStatus = ref();
const selectedToStatus = ref();

const availableFromStatuses = computed(() => {
    const result: WorkflowStatusVM[] = [];
    for(const status of props.workflow.statuses) {
        const fromTransitionsCount = props.workflow.transitions.filter(x => x.fromStatusId === status.id).length;
        if(fromTransitionsCount < props.workflow.statuses.length - 1) {
            result.push(status);
        }
    }
    return result;
})

const availableToStatuses = computed(() => {
    if(!selectedFromStatus.value) {
        return [];
    }

    const unavailableStatusesIds = [...new Set(props.workflow.transitions
        .filter(x => x.fromStatusId === selectedFromStatus.value)
        .map(x => x.toStatusId))];

    return props.workflow.statuses.filter(x => !unavailableStatusesIds.includes(x.id) && x.id !== selectedFromStatus.value);
})

function show() {
    dialog.value.show();
}

async function addTransition() {
    const model = new AddWorkflowTransitionDto();
    model.fromStatusId = selectedFromStatus.value;
    model.toStatusId = selectedToStatus.value;
    await workflowsService.addTransition(props.workflow.id, props.projectId, model);
    emit('onAdd');
}
</script>