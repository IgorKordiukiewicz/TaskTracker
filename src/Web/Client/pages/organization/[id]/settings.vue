<template>
    <div>
        <p class="text-lg">Settings</p>
        <div class="bg-white w-full shadow mt-4 p-4 flex flex-col gap-1" >
            <template v-if="canEditSettings && settings">
                <div class="flex justify-between items-center">
                    <div>
                        <p class="font-semibold mb-1">Name</p>
                        <p class="text-sm settings-label-caption">The name of the organization.</p>
                    </div>
                    <div class="flex items-center gap-2">
                        <InputText v-model="name" :disabled="!nameEditActive" class="w-80" />
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
                            <p class="font-semibold mb-1">Delete Organization</p>
                            <p class="text-sm settings-label-caption">This will permanently delete the organization and all its projects.</p>
                        </div>
                        <div >
                            <Button severity="danger" label="Delete" @click="deleteOrganization" />
                        </div>
                    </div>
                </template>
            </template>
            <template v-if="!isOwner">
                <Divider v-if="canEditSettings" />
                <div class="flex justify-between items-center">
                    <div>
                        <p class="font-semibold mb-1">Leave Organization</p>
                        <p class="text-sm settings-label-caption">Leave this organization and subsequently be removed from all its projects.</p>
                    </div>
                    <div >
                        <Button severity="danger" label="Leave" @click="leaveOrganization" />
                    </div>
                </div>
            </template>
        </div>
        <ConfirmDialog></ConfirmDialog>
    </div>
</template>

<script setup lang="ts">
import { UpdateOrganizationNameDto } from '~/types/dtos/organizations';
import { OrganizationPermissions } from '~/types/enums';

const route = useRoute();
const organizationsService = useOrganizationsService();
const confirm = useConfirm();
const permissions = usePermissions();

const organizationId = ref(route.params.id as string);
const settings = ref();
await updateSettings();

await permissions.checkOrganizationPermissions(organizationId.value);

const name = ref(settings.value?.name);
const nameEditActive = ref(false);

const canEditSettings = computed(() => {
    return permissions.hasPermission(OrganizationPermissions.EditOrganization);
})

const isOwner = computed(() => {
    return permissions.isOwner();
})

const updateNameSaveDisabled = computed(() => {
    return !name.value || name.value === settings.value?.name;
})

function activateNameEdit() {
    nameEditActive.value = true;
}

function cancelNameEdit() {
    nameEditActive.value = false;
    name.value = settings.value!.name;
}

async function updateSettings() {
    settings.value = await organizationsService.getSettings(organizationId.value);
}

async function updateName() {
    const model = new UpdateOrganizationNameDto();
    model.name = name.value!;
    await organizationsService.updateName(organizationId.value, model);
    await updateSettings();
    cancelNameEdit();
}

async function deleteOrganization() {
    confirm.require({
        message: `Are you sure you want to delete the ${settings.value!.name} organization?`,
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
            await organizationsService.deleteOrganization(organizationId.value);
            navigateTo('/');
        }
    })
}

async function leaveOrganization() {
    confirm.require({
        message: `Are you sure you want to leave the organization?`,
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
            await organizationsService.leaveOrganization(organizationId.value);
            navigateTo('/');
        }
    })
}
</script>