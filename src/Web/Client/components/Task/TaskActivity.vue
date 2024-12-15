<template>
    <div class="flex flex-col">
        <p v-html="description"></p>
        <p class="text-sm">
            {{ timeElapsed }}
        </p>
    </div>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import { TaskProperty } from '~/types/enums';
import type { TaskActivityVM } from '~/types/viewModels/tasks';

const props = defineProps({
    activity: { type: Object as PropType<TaskActivityVM>, required: true }
})

const timeParser = useTimeParser();

const timeElapsed = computed(() => {
    return timeParser.toReadableTimeDifference(props.activity.occurredAt);
});

const description = computed(() => {
    switch(props.activity.property) {
        case TaskProperty.Description:
            return "Updated description.";
        case TaskProperty.Status:
            return `Changed status from ${getValueDisplay(props.activity.oldValue)} to ${getValueDisplay(props.activity.newValue)}.`;
        case TaskProperty.Assignee:
            return !props.activity.newValue
                ? `Unassigned ${getValueDisplay(props.activity.oldValue)}.`
                : (!props.activity.oldValue ? `Assigned ${getValueDisplay(props.activity.newValue)}.` 
                : `Changed assignee from ${getValueDisplay(props.activity.oldValue)} to ${getValueDisplay(props.activity.newValue)}.`);
        case TaskProperty.Priority:
            return `Changed priority from ${getValueDisplay(props.activity.oldValue)} to ${getValueDisplay(props.activity.newValue)}.`;
        case TaskProperty.Title:
            return `Changed title from ${getValueDisplay(props.activity.oldValue)} to ${getValueDisplay(props.activity.newValue)}`;
        case TaskProperty.Creation:
            return 'Created.';
        default:
            return "";
    }
});

function getValueDisplay(value: string) {
    return `<span class="font-semibold">${value}</span>`;
}
</script>