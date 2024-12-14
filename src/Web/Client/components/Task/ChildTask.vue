<template>
    <li>
        <div class="flex items-center justify-between p-2  rounded-md" @contextmenu="menu.show($event)">
            <p :style="{ 'margin-left': levelMargin } " @click="navigateTo(`/project/${projectId}/tasks/${task.taskShortId}`)" class="cursor-pointer hover:font-medium">
                [#{{ task.taskShortId }}] {{ task.taskTitle }}
            </p>
            <i class="toggle pi pi-chevron-down cursor-pointer" :class="{ 'toggle-open': isOpen }" @click="toggleOpen" v-if="task.children.length > 0" />
            <ContextMenu ref="menu" :model="menuItems" />
        </div>
        <template v-if="isOpen">
            <ChildTask v-for="child in task.children" :task="child" :project-id="projectId" :level="level + 1" :can-edit-tasks="canEditTasks" />
        </template>
    </li>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import type { TaskHierarchyVM } from '~/types/viewModels/tasks';

const emit = defineEmits([ 'onRemove' ]);
const props = defineProps({
    task: { type: Object as PropType<TaskHierarchyVM>, required: true },
    projectId: { type: String, required: true },
    canEditTasks: { type: Boolean, required: true },
    level: { type: Number, required: false, default: 0 },
})

const confirm = useConfirm();
const isOpen = ref(true);
const menu = ref();

const menuItems = ref([
    {
        label: 'Remove Child',
        icon: 'pi pi-trash',
        disabled: !props.canEditTasks,
        command: () => {
            confirm.require({
                message: `Are you sure you want to remove the task ${props.task.taskTitle} as a child?`,
                header: 'Confirm action',
                rejectProps: {
                    label: 'Cancel',
                    severity: 'secondary'
                },
                acceptProps: {
                    label: 'Confirm',
                    severity: 'danger'
                },
                accept: () => emit('onRemove', props.task.taskId)
            })
        }
    }
])

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