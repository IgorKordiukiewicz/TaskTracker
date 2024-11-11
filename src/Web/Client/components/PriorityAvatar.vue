<template>
    <Avatar shape="circle" :icon="getIcon()" :style="{ 'background': getColor() }" class="text-white" />
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import { TaskPriority } from '~/types/enums';

const props = defineProps({
    priority: { type: Object as PropType<TaskPriority>, required: true }
});

function getIcon() {
    switch (+props.priority) {
        case TaskPriority.Urgent: 
            return "pi pi-angle-double-up";
        case TaskPriority.High:
            return "pi pi-angle-up";
        case TaskPriority.Normal:
            return "pi pi-minus";
        case TaskPriority.Low:
            return "pi pi-angle-down";
        default: 
            return "";
    }
}

function getColor() {
    switch (+props.priority) {
        case TaskPriority.Urgent: 
            return "var(--p-button-text-danger-color)";
        case TaskPriority.High:
            return "var(--p-button-text-warn-color)";
        case TaskPriority.Normal:
            return "var(--p-button-text-info-color)";
        case TaskPriority.Low:
            return "var(--p-button-text-success-color)";
        default: 
            return "#ffffff";
    }
}
</script>