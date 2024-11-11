<template>
    <div>
        <div class="flex justify-between items-center">
            <p class="text-lg">Tasks</p>
            <Button icon="pi pi-plus" severity="primary" label="Create" @click="openCreateTaskDialog" />
            <CreateTaskDialog v-if="members" ref="createTaskDialog" :projectId="projectId" :members="members" @on-create="updateTasks" />
        </div>
        <template v-if="tasks">
            <DataTable :value="tasks.tasks" class="mt-4 shadow" removable-sort>
                <Column header="Id" field="shortId" style="width: 80px;" sortable></Column>
                <Column header="Title" field="title" sortable></Column>
                <Column header="Status" style="width: 200px;" sortable sortField="status.name">
                    <template #body="slotProps">
                        {{ slotProps.data.status.name }}
                    </template>
                </Column>
                <Column header="Priority" style="width: 200px;" :sortable="true" sortField="priority">
                    <template #body="slotProps">
                        <div class="flex gap-3 items-center">
                            <PriorityAvatar :priority="slotProps.data.priority" />
                            <p>{{ TaskPriority[slotProps.data.priority] }}</p>
                        </div>
                    </template>
                </Column>
                <Column header="Assignee" style="width: 240px;" sortable sortField="assigneeId">
                    <template #body="slotProps">
                        <div class="flex gap-3 items-center" v-if="slotProps.data.assigneeId">
                            <Avatar label="AA" shape="circle" />
                            <p>Igor Kordiukiewicz</p>
                        </div>
                        <div v-else>
                            -
                        </div>
                    </template>
                </Column>
            </DataTable>
        </template>
    </div>
</template>

<script setup lang="ts">
import { TaskPriority } from '~/types/enums';

const route = useRoute();
const tasksService = useTasksService();
const projectsService = useProjectsService();

const createTaskDialog = ref();

const projectId = ref(route.params.id as string);
const tasks = ref(await tasksService.getTasks(projectId.value));
const members = ref(await projectsService.getMembers(projectId.value));

async function updateTasks() {
    tasks.value = await tasksService.getTasks(projectId.value);
}

function openCreateTaskDialog() {
    createTaskDialog.value.show();
}
</script>