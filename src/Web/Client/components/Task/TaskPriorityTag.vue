<template>
    <Tag class="w-28" :value="TaskPriority[priority]" :severity="getPrioritySeverity(priority)"></Tag>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import { TaskPriority } from '~/types/enums';

defineProps({
    priority: { type: Number as PropType<TaskPriority>, required: true }
})

function getPrioritySeverity(priority: TaskPriority) {
    switch (+priority) {
        case TaskPriority.Urgent: 
            return "danger";
        case TaskPriority.High:
            return "warn";
        case TaskPriority.Normal:
            return "info";
        case TaskPriority.Low:
            return "success";
        default: 
            return "primary";
    }
}
</script>