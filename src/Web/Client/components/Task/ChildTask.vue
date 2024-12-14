<template>
    <li>
        <div class="flex items-center justify-between p-2  rounded-md">
            <p :style="{ 'margin-left': levelMargin } " @click="navigateTo(`/project/${projectId}/tasks/${task.taskShortId}`)" class="cursor-pointer hover:font-medium">
                [#{{ task.taskShortId }}] {{ task.taskTitle }}
            </p>
            <i class="toggle pi pi-chevron-down cursor-pointer" :class="{ 'toggle-open': isOpen }" @click="toggleOpen" v-if="task.children.length > 0" />
        </div>
        <template v-if="isOpen">
            <ChildTask v-for="child in task.children" :task="child" :project-id="projectId" :level="level + 1" />
        </template>
    </li>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import type { TaskHierarchyVM } from '~/types/viewModels/tasks';

const props = defineProps({
    task: { type: Object as PropType<TaskHierarchyVM>, required: true },
    projectId: { type: String, required: true },
    level: { type: Number, required: false, default: 0 },
})

const isOpen = ref(true);

const levelMargin = computed(() => {
    return `${props.level * 16}px`;
});

function toggleOpen() {
    isOpen.value = !isOpen.value;
}
</script>

<style scoped>
.toggle {
    transition-property: transform;
    transition-duration: .3s;
    transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}

.toggle-open {
    transform: rotate(180deg);
}
</style>