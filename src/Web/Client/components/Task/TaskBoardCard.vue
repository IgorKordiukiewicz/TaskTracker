<template>
    <div class="p-3 shadow rounded-md bg-white text-md flex flex-col gap-2">
        <div class="flex justify-between items-start">
            <p>{{ task.title }}</p>
            <i class="pi pi-external-link cursor-pointer" @click="openTaskDetails" />
        </div>
        <p class="text-sm truncate text-ellipsis mb-1" v-if="task.description.length > 0">
            {{ task.description }}
        </p>
        <div class="flex justify-between items-end">
            <div class="flex gap-2 items-center">
                <Tag :severity="prioritySeverity" style="height: 30px; width: 30px; border-radius: 100%;">
                    <i class="pi pi-flag" />
                </Tag>
                <UserAvatar v-if="task.assigneeId" :user-id="task.assigneeId" style="height: 30px; width: 30px;"/>
                <Knob :model-value="timeKnobValue" :value-color="timeKnobColor" :value-template="(val) => ``" :stroke-width="10" readonly :size="30" v-if="task.estimatedTime" />
            </div>
            <div class="flex items-center gap-3">
                <i class="pi pi-align-left" v-if="hasDescription && false" />
                <div class="flex items-center gap-1" v-if="task.commentsCount > 0">
                    <i class="pi pi-comments" />
                    <p class="text-sm">{{ task.commentsCount }}</p>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { $dt } from '@primevue/themes';
import type { PropType } from 'vue';
import { TaskPriority } from '~/types/enums';
import type { TaskVM } from '~/types/viewModels/tasks';

const props = defineProps({
    projectId: { type: String, required: true },
    task: { type: Object as PropType<TaskVM>, required: true }
})

const prioritySeverity = computed(() => {
    switch (+props.task.priority) {
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
})

const hasDescription = computed(() => {
    return props.task.description.length > 0;
})

const timeKnobValue = computed(() => {
    if(!props.task.estimatedTime) {
        return 0;
    }

    return Math.min(Math.floor(props.task.totalTimeLogged / props.task.estimatedTime * 100), 100);
})

const timeKnobColor = computed(() => {
    if(!props.task.estimatedTime || props.task.totalTimeLogged < props.task.estimatedTime) {
        return $dt('knob.value.background').value;
    }

    return '#ef4444';
})

function openTaskDetails() {
    navigateTo(`/project/${props.projectId}/tasks/${props.task.shortId}`);
}
</script>