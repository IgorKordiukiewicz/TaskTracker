<template>
    <div class="flex flex-col">
        <p class="font-medium">
            {{ description }}
        </p>
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
            return `Changed status from ${props.activity.oldValue} to ${props.activity.newValue}.`;
        case TaskProperty.Assignee:
            return !props.activity.newValue
                ? `Unassigned ${props.activity.oldValue}.`
                : (!props.activity.oldValue ? `Assigned ${props.activity.newValue}.` : `Changed assignee from ${props.activity.oldValue} to ${props.activity.newValue}.`);
        case TaskProperty.Priority:
            return `Changed priority from ${props.activity.oldValue} to ${props.activity.newValue}.`
        default:
            return "";
    }
});
</script>