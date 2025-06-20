<template>
    <div>
        <div class="flex justify-between items-center" v-if="details">
            <div class="flex items-center gap-2">
                <p class="text-lg">
                    [#{{ taskShortId }}] {{ details.title }}
                </p>
            </div>
            <div class="flex gap-1 items-center">
                <Button icon="pi pi-chevron-down" severity="primary" label="Actions" @click="toggleActionsMenu" aria-haspopup="true" icon-pos="right" v-if="canEditTasks"  />
                <Menu ref="actionsMenu" :model="actionsMenuItems" :popup="true" />
                <Button icon="pi pi-chevron-left" text severity="secondary" label="Back" @click="navigateTo(`/project/${projectId}/`)" />
            </div>
            <UpdateTaskTitleDialog ref="updateTitleDialog" :id="details.id" :project-id="projectId" @on-update="updateDetails" />
            <ConfirmDialog></ConfirmDialog>
        </div>
        <div class="flex gap-4 w-full mt-4" v-if="details && members">
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
                        <Textarea v-model="descriptionEditValue" rows="5" :fluid="true" :auto-resize="true" style="resize: none;" :readonly="!canEditTasks" />
                    </div>

                </div>
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3" v-if="attachments">
                    <div class="flex justify-between items-center mb-2">
                        <div class="flex items-center gap-3 font-semibold">
                            <i class="pi pi-link" />
                            <p class="font-semibold">
                                Attachments
                            </p>
                        </div>
                        <div class="flex gap-3" v-if="canEditTasks">
                            <FileInput icon="pi pi-plus" file-types=".pdf,.txt,.csv,.jpg,.jpeg,.png,.gif,.json,.docx,.xlsx,.pptx" @upload="addAttachment" />
                        </div>
                    </div>
                    <div class="flex flex-col w-full" v-for="attachment in attachments.attachments">
                        <div class="flex items-center justify-between">
                            <div class="flex items-center gap-3">
                                <i :class="getAttachmentIcon(attachment.type)" />
                                <p>{{ attachment.name }}</p>
                            </div>
                            <div class="flex items-center gap-3">
                                <div class="text-sm">{{ filesize(attachment.bytesLength) }}</div>
                                <Button icon="pi pi-cloud-download" style="height: 24px; width: 24px;" text @click="downloadAttachment(attachment.name)" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3" v-if="comments">
                    <TogglableSection title="Comments" icon="pi pi-comments">
                        <div class="flex gap-2 items-center mt-1" v-if="canEditTasks">
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
                        <template v-if="canEditTasks">
                            <Select v-model="selectedStatusId" :options="statuses" option-label="name" option-value="id" class="w-full" @change="updateStatus" />
                        </template>
                        <template v-else>
                            <InputText readonly :value="readOnlyStatusDisplay" />
                        </template>
                    </LabeledInput>
                    <LabeledInput label="Priority">
                        <template v-if="canEditTasks">
                            <Select v-model="selectedPriority" :options="allTaskPriorities" option-label="name" option-value="key" class="w-full" @change="updatePriority" />
                        </template>
                        <template v-else>
                            <InputText readonly :value="readOnlyPriorityDisplay" />
                        </template>
                    </LabeledInput>
                    <LabeledInput label="Assignee">
                        <template v-if="canEditTasks">
                            <Select v-model="selectedAssigneeUserId" :options="members.members" option-label="name" option-value="userId" class="w-full" showClear @change="updateAssignee" />
                        </template>
                        <template v-else>
                            <InputText readonly :value="readOnlyAssigneeDisplay" />
                        </template>
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
                        <div class="flex gap-3" v-if="canEditTasks">
                            <Button severity="secondary" text icon="pi pi-plus" style="height: 24px; width: 24px;" @click="openLogTimeDialog"  />
                            <Button severity="secondary" text icon="pi pi-pencil" style="height: 24px; width: 24px;" @click="openEstimatedTimeDialog" />
                        </div>
                        <UpdateEstimatedTimeDialog ref="estimatedTimeDialog" @on-submit="updateEstimatedTime" />
                        <LogTimeDialog ref="logTimeDialog" @on-submit="addLoggedTime" />
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
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3" v-if="relationships">
                    <div class="flex justify-between items-center">
                        <div class="flex items-center gap-3 font-semibold">
                            <i class="pi pi-sitemap" />
                            <p class="font-semibold">
                                Relationships
                            </p>
                        </div>
                        <div class="flex gap-3" v-if="canEditTasks">
                            <Button severity="secondary" text icon="pi pi-plus" style="height: 24px; width: 24px;" @click="openAddChildDialog"  />
                        </div>
                        <AddChildDialog ref="addChildDialog" @on-submit="addChild" :task-id="details.id" :project-id="projectId" />
                    </div>
                    <div class="flex flex-col gap-1" v-if="relationships.parent">
                        <label class="text-sm">Parent</label>
                        <div class="p-2 rounded-md select-border flex items-center">
                            <p class="cursor-pointer hover:font-medium text-sm" @click="navigateTo(`/project/${projectId}/tasks/${relationships.parent.shortId}`)">
                                [#{{ relationships.parent.shortId }}] {{ relationships.parent.title }}
                            </p>
                        </div>
                    </div>
                    <div class="flex flex-col gap-1"  v-if="relationships.childrenHierarchy">
                        <label class="text-sm">Children</label>
                        <ul class="rounded-md select-border text-sm">
                            <ChildTask  v-for="task in relationships.childrenHierarchy.children" 
                            :task="task" :project-id="projectId" :can-edit-tasks="canEditTasks" @on-remove="removeChild" />
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { $dt } from '@primevue/themes';
import type { TreeNode } from 'primevue/treenode';
import UpdateEstimatedTimeDialog from '~/components/Task/UpdateEstimatedTimeDialog.vue';
import { AddTaskCommentDto, AddTaskLoggedTimeDto, AddTaskRelationshipDto, RemoveTaskRelationshipDto, UpdateTaskAssigneeDto, UpdateTaskDescriptionDto, UpdateTaskEstimatedTimeDto, UpdateTaskPriorityDto, UpdateTaskStatusDto } from '~/types/dtos/tasks';
import { allTaskPriorities, AttachmentType, ProjectPermissions, TaskPriority } from '~/types/enums';
import type { TaskHierarchyVM, TaskVM } from '~/types/viewModels/tasks';
import { filesize } from 'filesize';

const route = useRoute();
const tasksService = useTasksService();
const projectsService = useProjectsService();
const timeParser = useTimeParser();
const permissions = usePermissions();
const confirm = useConfirm();

const projectId = ref(route.params.id as string);
const taskShortId = ref(+(route.params.shortId as string));
const details = ref<TaskVM | undefined>();
const members = ref(await projectsService.getMembers(projectId.value)); // TODO: pass from tasks list page?
const comments = ref();
const activities = ref();
const relationships = ref();
const attachments = ref();
await updateDetails();
await updateComments();
await updateRelationships();
await updateAttachments();

await permissions.checkProjectPermissions(projectId.value);

const logTimeDialog = ref();
const estimatedTimeDialog = ref();
const updateTitleDialog = ref();
const actionsMenu = ref();
const addChildDialog = ref();

const statuses = computed(() => {
    if(!details.value) {
        return [];
    }

    return details.value.possibleNextStatuses.map(x => ({
        id: x.id, name: x.name }))
        .concat([
        { id: details.value.status.id, name: details.value.status.name }
    ]);
});

const actionsMenuItems = ref([
    { 
        label: 'Edit Title',
        icon: 'pi pi-pencil',
        command: () => {
            updateTitleDialog.value.show(details.value!.title);
        }
    },
    {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => {
            confirm.require({
                message: `Are you sure you want to delete the task?`,
                header: 'Confirm action',
                rejectProps: {
                    label: 'Cancel',
                    severity: 'secondary'
                },
                acceptProps: {
                    label: 'Confirm',
                    severity: 'danger'
                },
                accept: async () => await deleteTask()
            })
        }
    }
])

const descriptionEditValue = ref(details.value?.description);
const selectedPriority = ref(details.value?.priority);
const selectedAssigneeUserId = ref(details.value?.assigneeId);
const selectedStatusId = ref(details.value?.status.id);
const newCommentContent = ref();

const canEditTasks = computed(() => {
    return permissions.hasPermission(ProjectPermissions.EditTasks);
})

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

const readOnlyStatusDisplay = computed(() => {
    return details.value!.status.name;
})

const readOnlyPriorityDisplay = computed(() => {
    return TaskPriority[details.value!.priority];
})

const readOnlyAssigneeDisplay = computed(() => {
    return members.value!.members.find(x => x.id === details.value!.assigneeId)?.name ?? '-';
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

function toggleActionsMenu(event: Event) {
    actionsMenu.value.toggle(event);
}

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

async function updateRelationships() {
    relationships.value = details.value 
        ? await tasksService.getRelationships(details.value.id, projectId.value)
        : null;
}

async function updateAttachments() {
    attachments.value = details.value
        ? await tasksService.getAttachments(details.value.id, projectId.value)
        : null;
}

function cancelDescriptionEdit() {
    descriptionEditValue.value = details.value!.description;
}

function openLogTimeDialog() {
    logTimeDialog.value.show();
}

function openEstimatedTimeDialog() {
    estimatedTimeDialog.value.show(details.value!.estimatedTime ? timeParser.fromMinutes(details.value!.estimatedTime) : null);
}

async function openAddChildDialog() {
    await addChildDialog.value.show();
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

async function addLoggedTime(model: AddTaskLoggedTimeDto) {
    await tasksService.addLoggedTime(details.value!.id, projectId.value, model);
    await updateDetails();
}

async function deleteTask() {
    await tasksService.deleteTask(details.value!.id, projectId.value);
    navigateTo(`/project/${projectId.value}`);
}

async function addChild(model: AddTaskRelationshipDto) {
    await tasksService.addTaskRelationship(projectId.value, model);
    await updateRelationships();
}

async function removeChild(childId: string) {
    const model = new RemoveTaskRelationshipDto();
    model.parentId = details.value!.id;
    model.childId = childId;
    await tasksService.removeTaskRelationship(projectId.value, model);
    await updateRelationships();
}

async function addAttachment(file: File) {
    await tasksService.addAttachment(details.value!.id, projectId.value, file);
    await updateAttachments();
}

async function downloadAttachment(attachmentName: string) {
    const downloadUrl = await tasksService.downloadAttachment(details.value!.id, projectId.value, attachmentName);
    window.open(downloadUrl!, "_blank");
}

function getAttachmentIcon(type: AttachmentType) {
    switch(+type) {
        case AttachmentType.Document:
            return "pi pi-file";
        case AttachmentType.Image:
            return "pi pi-image";
        default:
            return "pi pi-file";   
    }
}
</script>

<style scoped>
.negativeMargin {
    margin-bottom: -18px;
}
</style>