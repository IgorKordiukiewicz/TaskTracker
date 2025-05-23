<template>
    <div v-if="canViewPage">
        <p class="text-lg">Settings</p>
        <div class="bg-white w-full shadow mt-4 p-4 flex flex-col gap-1" v-if="settings">
            <div class="flex justify-between items-center">
                <div>
                    <p class="font-semibold mb-1">Name</p>
                    <p class="text-sm settings-label-caption">The name of the project.</p>
                </div>
                <div class="flex items-center gap-2">
                    <InputText v-model="projectName" :disabled="!nameEditActive" class="w-80" />
                    <template v-if="nameEditActive">
                        <Button severity="secondary" text icon="pi pi-times" @click="cancelNameEdit" />
                        <Button severity="primary" text icon="pi pi-check" @click="updateName" :disabled="updateNameSaveDisabled" />
                    </template>
                    <template v-else>
                        <Button severity="secondary" text icon="pi pi-pencil" @click="activateNameEdit" />
                    </template>
                </div>
            </div>
            <template v-if="isOwner">
                <Divider />
                <div class="flex justify-between items-center">
                    <div>
                        <p class="font-semibold mb-1">Delete Project</p>
                        <p class="text-sm settings-label-caption">This will permanently delete the project.</p>
                    </div>
                    <div >
                        <Button severity="danger" label="Delete" @click="deleteProject" />
                    </div>
                </div>
            </template>
            <template v-else>
                <!-- TODO: Leave project -->
            </template>
        </div>
        <ConfirmDialog></ConfirmDialog>
    </div>
</template>

<script setup lang="ts">
import { UpdateProjectNameDto } from '~/types/dtos/projects';
import { ProjectPermissions } from '~/types/enums';

const route = useRoute();
const projectsService = useProjectsService();
const confirm = useConfirm();
const permissions = usePermissions();

const projectId = ref(route.params.id as string);
const settings = ref();
await updateSettings();

await permissions.checkProjectPermissions(projectId.value);

const projectName = ref(settings.value?.name);
const nameEditActive = ref(false);

const canViewPage = computed(() => {
    return permissions.hasPermission(ProjectPermissions.EditProject);
})

const updateNameSaveDisabled = computed(() => {
    return !projectName.value || projectName.value === settings.value?.name;
})

const isOwner = computed(() => {
    return permissions.isOwner();
})

async function updateSettings() {
    settings.value = await projectsService.getSettings(projectId.value);
}

function activateNameEdit() {
    nameEditActive.value = true;
}

function cancelNameEdit() {
    nameEditActive.value = false;
    projectName.value = settings.value!.name;
}

async function updateName() {
    const model = new UpdateProjectNameDto();
    model.name = projectName.value!;
    await projectsService.updateName(projectId.value, model);
    await updateSettings();
    cancelNameEdit();
}

async function deleteProject() {
    confirm.require({
        message: `Are you sure you want to delete the ${settings.value!.name} project?`,
        header: 'Confirm action',
        rejectProps: {
            label: 'Cancel',
            severity: 'secondary'
        },
        acceptProps: {
            label: 'Confirm',
            severity: 'danger'
        },
        accept: async () => {
            await projectsService.deleteProject(projectId.value);
            navigateTo('/');
        }
    })
}
</script>