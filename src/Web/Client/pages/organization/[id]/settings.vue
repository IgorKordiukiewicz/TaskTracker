<template>
    <div>
        <p class="text-lg">Settings</p>
        <div class="bg-white w-full shadow mt-4 p-4 flex flex-col gap-1" v-if="settings">
            <div class="flex justify-between items-center">
                <div>
                    <p class="font-semibold mb-1">Name</p>
                    <p style="color: rgb(100, 116, 139);" class="text-sm">The name of the organization.</p>
                </div>
                <div class="flex items-center gap-2">
                    <InputText v-model="name" :disabled="!nameEditActive" class="w-80" />
                    <template v-if="nameEditActive">
                        <Button severity="secondary" text icon="pi pi-times" @click="cancelNameEdit" />
                        <Button severity="primary" text icon="pi pi-check" @click="updateName" :disabled="!name" />
                    </template>
                    <template v-else>
                        <Button severity="secondary" text icon="pi pi-pencil" @click="activateNameEdit" />
                    </template>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { UpdateOrganizationNameDto } from '~/types/dtos/organizations';

const route = useRoute();
const organizationsService = useOrganizationsService();

const organizationId = ref(route.params.id as string);
const settings = ref(await organizationsService.getSettings(organizationId.value));

const name = ref(settings.value?.name);
const nameEditActive = ref(false);

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
</script>