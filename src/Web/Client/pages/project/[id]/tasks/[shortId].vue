<template>
    <div>
        <div class="flex justify-between items-center" v-if="details">
            <div class="flex items-center gap-2">
                <p class="text-lg">
                    [#{{ taskShortId }}] {{ details.title }}
                </p>
            </div>
            <div class="flex gap-1 items-center">
                <Button icon="pi pi-pencil" text severity="secondary" label="Title" @click="openUpdateTitleDialog" />
                <Button icon="pi pi-chevron-left" text severity="secondary" label="Back" @click="navigateTo(`/project/${projectId}/`)" />
            </div>
            <UpdateTaskTitleDialog ref="updateTitleDialog" :id="details.id" :project-id="projectId" @on-update="updateDetails" />
        </div>
        <div class="flex gap-4 w-100 mt-4" v-if="details && members">
            <div class="flex flex-col gap-4 w-3/4">
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3">
                    <div class="flex justify-between items-center">
                        <div class="flex items-center gap-3">
                            <i class="pi pi-align-left" />
                            <p class="font-semibold">
                                Description
                            </p>
                        </div>
                        <div v-if="descriptionEditValue !== details.description" class="flex gap-3">
                            <Button severity="secondary" text icon="pi pi-times" style="height: 24px; width: 24px;" @click="cancelDescriptionEdit" />
                            <Button severity="primary" text icon="pi pi-check" :disabled="!descriptionEditValue" style="height: 24px; width: 24px;" @click="updateDescription" />
                        </div>
                    </div>
                    <div>
                        <Textarea v-model="descriptionEditValue" rows="5" :fluid="true" :auto-resize="true" style="resize: none;" />
                    </div>

                </div>
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3" v-if="comments">
                    <TogglableSection title="Comments" icon="pi pi-comments">
                        <div class="flex gap-2 items-center mt-1">
                            <InputText v-model="newCommentContent" class="w-full" placeholder="Add a comment" />
                            <Button icon="pi pi-send" label="Send" :disabled="!newCommentContent" @click="addComment" />
                        </div>
                        <ScrollPanel class="h-fit" style="max-height: 50vh;" id="commentsScroll">
                            <div class="flex flex-col gap-4 mt-2" :class="{ negativeMargin: isScrollHidden('commentsScroll', 'commentsContent') }" id="commentsContent">
                                <TaskComment v-for="comment in comments.comments" :comment="comment" />
                            </div>
                        </ScrollPanel>
                    </TogglableSection>
                </div>
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3" v-if="activities">
                    <TogglableSection title="Activity" icon="pi pi-history">
                        <ScrollPanel class="h-fit" style="max-height: 50vh;" id="activitiesScroll">
                            <div class="flex flex-col gap-3 mt-2" :class="{ negativeMargin: isScrollHidden('activitiesScroll', 'activitiesContent') }" id="activitiesContent">
                                <TaskActivity v-for="activity in activities.activities" :activity="activity" />
                            </div>
                        </ScrollPanel>
                    </TogglableSection>
                </div>
            </div>
            <div class="flex flex-col gap-4 w-1/4">
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3 text-sm">
                    <div class="flex items-center gap-3 font-semibold text-base">
                        <i class="pi pi-list" /> <!-- pi-hashtag -->
                        <p class="font-semibold">
                            Properties
                        </p>
                    </div>
                    <LabeledInput label="Status">
                        <Select v-model="selectedStatusId" :options="statuses" option-label="name" option-value="id" class="w-full" @change="updateStatus" />
                    </LabeledInput>
                    <LabeledInput label="Priority">
                        <Select v-model="selectedPriority" :options="priorities" option-label="name" option-value="key" class="w-full" @change="updatePriority" />
                    </LabeledInput>
                    <LabeledInput label="Assignee">
                        <Select v-model="selectedAssigneeUserId" :options="members.members" option-label="name" option-value="userId" class="w-full" showClear @change="updateAssignee" />
                    </LabeledInput>
                </div>
                <div class="bg-white w-full shadow p-4">
                    <div class="flex justify-between items-center">
                        <div class="flex items-center gap-3">
                            <i class="pi pi-stopwatch" />
                            <p class="font-semibold">
                                Time Tracking
                            </p>
                        </div>
                        <div class="flex gap-3">
                            <Button severity="secondary" text icon="pi pi-plus" style="height: 24px; width: 24px;" @click="openLogTimeDialog"  />
                            <Button severity="secondary" text icon="pi pi-pencil" style="height: 24px; width: 24px;" @click="openEstimatedTimeDialog" />
                        </div>
                        <TimeInputDialog ref="estimatedTimeDialog" header="Edit estimated time" @on-submit="updateEstimatedTime" />
                        <TimeInputDialog ref="logTimeDialog" header="Log time" @on-submit="addLoggedTime" />
                    </div>
                    <div class="flex gap-2 items-center w-full mt-4">
                        <div class="flex flex-col gap-1 items-center w-full">
                            <p class="text-sm">Logged</p>
                            <p class="text-base font-semibold">{{ loggedTimeDisplay }}</p>
                        </div>
                        <template v-if="details.estimatedTime">
                            <Knob :model-value="timeKnobValue" :value-template="(val) => `${val}%`" :stroke-width="10" readonly 
                                :value-color="timeKnobColor" v-tooltip.bottom="remainingTimeDisplay" />
                        </template>
                        <template v-else>
                            <Knob :model-value="0" :value-template="(val) => `-`" :stroke-width="10" readonly />
                        </template>
                        <div class="flex flex-col gap-1 items-center w-full">
                            <p class="text-sm">Estimated</p>
                            <p class="text-base font-semibold">{{ estimatedTimeDisplay }}</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { $dt } from '@primevue/themes';
import { AddTaskCommentDto, AddTaskLoggedTimeDto, UpdateTaskAssigneeDto, UpdateTaskDescriptionDto, UpdateTaskEstimatedTimeDto, UpdateTaskPriorityDto, UpdateTaskStatusDto } from '~/types/dtos/tasks';
import { TaskPriority } from '~/types/enums';

const route = useRoute();
const tasksService = useTasksService();
const projectsService = useProjectsService();
const timeParser = useTimeParser();

const projectId = ref(route.params.id as string);
const taskShortId = ref(+(route.params.shortId as string));
const details = ref(await tasksService.getTask(taskShortId.value, projectId.value));
const members = ref(await projectsService.getMembers(projectId.value)); // TODO: pass from tasks list page?
const comments = ref(details.value 
    ? await tasksService.getComments(details.value.id, projectId.value)
    : null);
const activities = ref(details.value 
    ? await tasksService.getActivities(details.value.id, projectId.value)
    : null);

const logTimeDialog = ref();
const estimatedTimeDialog = ref();
const updateTitleDialog = ref();

const priorities = ref([
    { key: TaskPriority.Low, name: TaskPriority[TaskPriority.Low] },
    { key: TaskPriority.Normal, name: TaskPriority[TaskPriority.Normal] },
    { key: TaskPriority.High, name: TaskPriority[TaskPriority.High] },
    { key: TaskPriority.Urgent, name: TaskPriority[TaskPriority.Urgent] },
]);

const statuses = ref(details.value?.possibleNextStatuses.map(x => ({
    id: x.id, name: x.name }))
    .concat([
        { id: details.value.status.id, name: details.value.status.name }
    ]
))

const descriptionEditValue = ref(details.value?.description);
const selectedPriority = ref(details.value?.priority);
const selectedAssigneeUserId = ref(details.value?.assigneeId);
const selectedStatusId = ref(details.value?.status.id);
const newCommentContent = ref();

const loggedTimeDisplay = computed(() => {
    return details.value ? timeParser.fromMinutes(details.value?.totalTimeLogged) : '';
})

const estimatedTimeDisplay = computed(() => {
    return details.value?.estimatedTime ? timeParser.fromMinutes(details.value.estimatedTime) : '-';
})

const remainingTimeDisplay = computed(() => {
    return details.value?.estimatedTime
        ? `Remaining: ${timeParser.fromMinutes(details.value.estimatedTime - details.value.totalTimeLogged)}`
        : '-';
})

const timeKnobValue = computed(() => {
    if(!details.value || !details.value.estimatedTime) {
        return 0;
    }

    return Math.min(Math.floor(details.value.totalTimeLogged / details.value.estimatedTime * 100), 100);
})

const timeKnobColor = computed(() => {
    if(!details.value || !details.value.estimatedTime || details.value.totalTimeLogged < details.value.estimatedTime) {
        return $dt('knob.value.background').value;
    }

    return '#ef4444';
})

function isScrollHidden(scrollPanelId: string, contentId: string) {
    const scrollPanel = document.getElementById(scrollPanelId);
    const content = document.getElementById(contentId);
    return scrollPanel && content ? content.scrollHeight < scrollPanel.clientHeight : true;
}

async function updateDetails() {
    details.value = await tasksService.getTask(taskShortId.value, projectId.value);
    activities.value = details.value 
        ? await tasksService.getActivities(details.value.id, projectId.value)
        : null;
}

async function updateComments() {
    comments.value = details.value
        ? await tasksService.getComments(details.value.id, projectId.value)
        : null;
}

function cancelDescriptionEdit() {
    descriptionEditValue.value = details.value!.description;
}

function openLogTimeDialog() {
    logTimeDialog.value.show();
}

function openEstimatedTimeDialog() {
    estimatedTimeDialog.value.show();
}

function openUpdateTitleDialog() {
    updateTitleDialog.value.show(details.value!.title);
}

async function updateDescription() {
    const model = new UpdateTaskDescriptionDto();
    model.description = descriptionEditValue.value!;
    await tasksService.updateDescription(details.value!.id, projectId.value, model);
    await updateDetails();
}

async function updatePriority() {
    if(selectedPriority.value === details.value!.priority) {
        return;
    }

    const model = new UpdateTaskPriorityDto();
    model.priority = selectedPriority.value!;
    await tasksService.updatePriority(details.value!.id, projectId.value, model);
    await updateDetails();
}

async function updateAssignee() {
    if(selectedAssigneeUserId.value === details.value!.assigneeId) {
        return;
    }

    const memberId = selectedAssigneeUserId.value 
        ? members.value!.members.find(x => x.userId === selectedAssigneeUserId.value)!.id
        : undefined;
    const model = new UpdateTaskAssigneeDto();
    model.memberId = memberId;
    await tasksService.updateAssignee(details.value!.id, projectId.value, model);
    await updateDetails();
}

async function updateStatus() {
    if(selectedStatusId.value === details.value!.status.id) {
        return;
    }

    const model = new UpdateTaskStatusDto();
    model.statusId = selectedStatusId.value!;
    await tasksService.updateStatus(details.value!.id, projectId.value, model);
    await updateDetails();
}

async function addComment() {
    const model = new AddTaskCommentDto();
    model.content = newCommentContent.value;
    newCommentContent.value = '';
    await tasksService.addComment(details.value!.id, projectId.value, model);
    await updateComments();
}

async function updateEstimatedTime(minutes: number) {
    const model = new UpdateTaskEstimatedTimeDto();
    model.minutes = minutes;
    await tasksService.updateEstimatedTime(details.value!.id, projectId.value, model);
    await updateDetails();
}

async function addLoggedTime(minutes: number) {
    const model = new AddTaskLoggedTimeDto();
    model.minutes = minutes;
    await tasksService.addLoggedTime(details.value!.id, projectId.value, model);
    await updateDetails();
}
</script>

<style scoped>
.negativeMargin {
    margin-bottom: -18px;
}
</style>