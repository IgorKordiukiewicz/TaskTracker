<template>
    <div>
        <div class="flex justify-between items-center">
            <p class="text-lg">Tasks</p>
            <div class="flex gap-2 items-center">
                <Button type="button" icon="pi pi-filter-slash" label="Clear" severity="contrast" outlined @click="resetFilters" />
                <IconField>
                    <InputIcon class="pi pi-search" />
                    <InputText v-model="filters['global'].value" placeholder="Search" />
                </IconField>
                <Button icon="pi pi-plus" severity="primary" label="Create" @click="openCreateTaskDialog" v-if="canCreateTasks" />
                <CreateTaskDialog v-if="members" ref="createTaskDialog" :projectId="projectId" :members="members" @on-create="updateTasks" />
            </div>
        </div>
        <template v-if="tasks && members">
            <DataTable :value="tasks.tasks" class="mt-4 shadow" removable-sort data-key="shortId" :row-hover="true" :row-class="() => 'cursor-pointer'"
            filter-display="menu" :global-filter-fields="['title' ,'description' ]" v-model:filters="filters" @row-click="openTaskDetails" paginator :rows="10" :rows-per-page-options="[10, 25, 50]">
                <Column header="Id" field="shortId" style="width: 80px;" sortable filter-field="shortId" data-type="numeric" :show-filter-match-modes="false">
                    <template #body="{ data }">
                        {{  data.shortId }}
                    </template>
                    <template #filter="{ filterModel }">
                        <InputNumber v-model="filterModel.value" />
                    </template>
                </Column>
                <Column header="Title" field="title" sortable style="width: 300px;">
                    <template #body="{ data }">
                        {{ data.title }}
                    </template>
                    <template #filter="{ filterModel }">
                        <InputText v-model="filterModel.value" type="text" placeholder="Search by title" />
                    </template>
                </Column>
                <Column header="Description" field="description">
                    <template #body="{ data }">
                        {{ data.description }}
                    </template>
                    <template #filter="{ filterModel }">
                        <InputText v-model="filterModel.value" type="text" placeholder="Search by description" />
                    </template>
                </Column>
                <Column header="Status" style="width: 200px;" sortable sortField="status.name" filter-field="status.name" :show-filter-match-modes="false">
                    <template #body="{ data }">
                        <Tag class="w-28" :value="data.status.name" severity="secondary"></Tag>
                    </template>
                    <template #filter="{ filterModel }">
                        <MultiSelect v-model="filterModel.value" :options="tasks.allTaskStatuses" option-value="name" option-label="name"></MultiSelect>
                    </template>
                </Column>
                <Column header="Priority" style="width: 200px;" sortable sortField="priority" filter-field="priority" :show-filter-match-modes="false">
                    <template #body="{ data }">
                        <TaskPriorityTag :priority="data.priority" />
                    </template>
                    <template #filter="{ filterModel }">
                        <MultiSelect v-model="filterModel.value" :options="allTaskPriorities" option-label="name" option-value="key"></MultiSelect>
                    </template>
                </Column>
                <Column header="Assignee" style="width: 240px;" sortable sortField="assigneeId" filter-field="assigneeId" :show-filter-match-modes="false">
                    <template #body="{ data }">
                        <div class="flex gap-3 items-center" v-if="data.assigneeId">
                            <UserAvatar :user-id="data.assigneeId" />
                            <p>{{ getMemberName(data.assigneeId) }}</p>
                        </div>
                        <div v-else>
                            -
                        </div>
                    </template>
                    <template #filter="{ filterModel }">
                        <MultiSelect v-model="filterModel.value" :options="members.members" option-label="name" option-value="userId"></MultiSelect>
                    </template>
                </Column>
            </DataTable>
        </template>
    </div>
</template>

<script setup lang="ts">
import { allTaskPriorities, ProjectPermissions, TaskPriority } from '~/types/enums';
import { FilterMatchMode } from '@primevue/core/api';
import type { DataTableRowClickEvent } from 'primevue/datatable';

const route = useRoute();
const tasksService = useTasksService();
const projectsService = useProjectsService();
const permissions = usePermissions();

const createTaskDialog = ref();

const projectId = ref(route.params.id as string);
const tasks = ref();
await updateTasks();
const members = ref(await projectsService.getMembers(projectId.value));

await permissions.checkProjectPermissions(projectId.value);

const filters = ref();

const canCreateTasks = computed(() => {
    return permissions.hasPermission(ProjectPermissions.EditTasks);
})

async function updateTasks() {
    tasks.value = await tasksService.getTasks(projectId.value);
}

function openCreateTaskDialog() {
    createTaskDialog.value.show();
}

function getMemberName(userId: string) {
    return members.value?.members.find(x => x.userId === userId)?.name;
}

const initFilters = () => {
    filters.value = {
        global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        shortId: { value: null, matchMode: FilterMatchMode.EQUALS },
        title: { value: null, matchMode: FilterMatchMode.CONTAINS },
        description: { value: null, matchMode: FilterMatchMode.CONTAINS },
        'status.name': { value: null, matchMode: FilterMatchMode.IN },
        priority: { value: null, matchMode: FilterMatchMode.IN },
        assigneeId: { value: null, matchMode: FilterMatchMode.IN }
    };
}
initFilters();

function resetFilters() {
    initFilters();
}

function openTaskDetails(event: DataTableRowClickEvent) {
    navigateTo(`/project/${projectId.value}/tasks/${event.data.shortId}`);
}
</script>