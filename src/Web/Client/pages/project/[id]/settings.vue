<template>
    <div>
        <p class="text-lg">Settings</p>
        <div class="bg-white w-full shadow mt-4 p-4 flex flex-col gap-1" v-if="settings">
            <div class="flex justify-between items-center">
                <div>
                    <p class="font-semibold mb-1">Name</p>
                    <p style="color: rgb(100, 116, 139);" class="text-sm">The name of the project.</p>
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
            <Divider />
            <div class="flex justify-between items-center">
                <div>
                    <p class="font-semibold mb-1">Delete Project</p>
                    <p style="color: rgb(100, 116, 139);" class="text-sm">This will permanently delete the project.</p>
                </div>
                <div >
                    <Button severity="danger" label="Delete" @click="deleteProject" />
                </div>
            </div>
        </div>
        <ConfirmDialog></ConfirmDialog>
    </div>
</template>

<script setup lang="ts">
import { UpdateProjectNameDto } from '~/types/dtos/projects';

const route = useRoute();
const projectsService = useProjectsService();
const confirm = useConfirm();

const projectId = ref(route.params.id as string);
const settings = ref(await projectsService.getSettings(projectId.value));

const projectName = ref(settings.value?.name);
const nameEditActive = ref(false);

const updateNameSaveDisabled = computed(() => {
    return !projectName.value || projectName.value === settings.value?.name;
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
            const navData = await projectsService.getNavData(projectId.value);
            await projectsService.deleteProject(projectId.value);
            if(navData) {
                navigateTo(`/organization/${navData?.organization.id}`);
            }
            else {
                navigateTo('/');
            }
        }
    })
}
</script>