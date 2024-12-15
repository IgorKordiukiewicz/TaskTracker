<template>
    <div class="h-full">
        <div class="flex justify-between items-center">
            <div class="flex items-center gap-4">
                <p class="text-lg">Tasks</p>
                <SelectButton v-model="view" :options="options" option-label="value" data-key="value">
                    <template #option="slotProps">
                        <i :class="slotProps.option.icon" />
                    </template>
                </SelectButton>
            </div>
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
            <template v-if="view.value === 'List'">
                <DataTable :value="tasks.tasks" class="mt-4 shadow" removable-sort data-key="shortId" :row-hover="true" :row-class="() => 'cursor-pointer'"
                filter-display="menu" :global-filter-fields="['title' ,'description' ]" v-model:filters="filters" @row-click="openTaskDetails" 
                paginator :rows="10" :rows-per-page-options="[10, 25, 50]" :always-show-paginator="false">
                    <Column header="Id" field="shortId" style="width: 80px;" sortable filter-field="shortId" data-type="numeric" :show-filter-match-modes="false">
                        <template #body="{ data }">
                            {{  data.shortId }}
                        </template>
                        <template #filter="{ filterModel }">
                            <InputNumber v-model="filterModel.value" />
                        </template>
                    </Column>
                    <Column header="Title" field="title" sortable>
                        <template #body="{ data }">
                            {{ data.title }}
                        </template>
                        <template #filter="{ filterModel }">
                            <InputText v-model="filterModel.value" type="text" placeholder="Search by title" />
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
            <template v-else>
                <div class="mt-4 flex gap-4 overflow-y-auto" style="height: calc(100% - 3.6rem); width: calc(100vw - 309px);"> <!-- Doesn't work on really small viewport widths -->
                    <div v-for="column in tasks.boardColumns" class="p-3 rounded-lg bg-surface-300 min-w-80 max-w-80 h-full border-0"  style="background: rgba(255, 255, 255, 0)"
                    :style="{ 'border-color': getDragColor(column) }">
                        <p class="text-lg mb-2 bg-white shadow-md rounded-md p-3 font-semibold text-center sticky top-0" style="z-index: 1;">{{ column.statusName }}</p>
                        <div class="mb-2 rounded-sm sticky top-0" style="height: 3px; z-index: 1;" :style="{ 'background': getDragColor(column) }"></div>
                        <draggable :list="column.tasksIds" class="flex flex-col gap-2" style="height: calc(100% - 4rem)" :group="{ name: 'statuses', put: isTransitionAllowed(column) }" v-bind="dragOptions" 
                        @start="() => onTaskDragStart(column.statusId)" @end="onTaskDragEnd" @change="(e) => onBoardChanged(column, e)">
                            <template #item="{element}">
                                <TaskBoardCard :project-id="projectId":task="tasks.tasks.find(x => x.id === element)!" />
                            </template>
                        </draggable>
                    </div>
                </div>
            </template>
        </template>
    </div>
</template>

<script setup lang="ts">
import { allTaskPriorities, ProjectPermissions, TaskPriority } from '~/types/enums';
import { FilterMatchMode } from '@primevue/core/api';
import type { DataTableRowClickEvent } from 'primevue/datatable';
import draggable from 'vuedraggable';
import type { TaskBoardColumnVM, TasksVM } from '~/types/viewModels/tasks';
import { UpdateTaskBoardColumnDto, UpdateTaskBoardDto, UpdateTaskStatusDto } from '~/types/dtos/tasks';

const route = useRoute();
const tasksService = useTasksService();
const projectsService = useProjectsService();
const permissions = usePermissions();

const createTaskDialog = ref();

const view = ref({ icon: 'pi pi-list', value: 'List' });
const options = ref([
    { icon: 'pi pi-list', value: 'List' },
    { icon: 'pi pi-objects-column', value: 'Board' }
])

const dragOptions = ref({
    animation: 200,
    ghostClass: "ghost"
})
const currentDragStatusId = ref();

const projectId = ref(route.params.id as string);
const tasks = ref<TasksVM | null | undefined>();
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

function onTaskDragStart(statusId: string) {
    currentDragStatusId.value = statusId;
}

function onTaskDragEnd() {
    currentDragStatusId.value = undefined;
}

async function onBoardChanged(column: TaskBoardColumnVM, event: any) {
    let update = false;
    if (event.added) {
        const model = new UpdateTaskStatusDto();
        model.statusId = column.statusId;
        await tasksService.updateStatus(event.added.element, projectId.value, model);
        await updateTasks();
    }
    else if (event.moved) {
        update = true;
    }

    if(!update) {
        return;
    }

    const updateModel = new UpdateTaskBoardDto();
    updateModel.projectId = projectId.value;
    updateModel.columns = tasks.value!.boardColumns.map(x => new UpdateTaskBoardColumnDto(x.statusId, x.tasksIds));
    await tasksService.updateBoard(projectId.value, updateModel);
}

function getDragColor(column: TaskBoardColumnVM) {
    if(!currentDragStatusId.value || column.statusId === currentDragStatusId.value) {
        return "#cbd5e1";
    }

    if(column.possibleNextStatuses.includes(currentDragStatusId.value)) {
        return "#10b981bb";
    }
    else {
        return "#ef4444bb";
    }
}

function isTransitionAllowed(column: TaskBoardColumnVM) {
    if(!currentDragStatusId.value) {
        return true;
    }

    if(column.possibleNextStatuses.includes(currentDragStatusId.value) || column.statusId === currentDragStatusId.value) {
        return true;
    }

    return false;
}
</script>

<style scoped>
.ghost {
  opacity: 0.4;
}
</style>