<template>
    <div>
        <div class="flex justify-between items-center">
            <p class="text-lg">Organizations</p>
            <Button icon="pi pi-plus" severity="primary" label="Create" @click="openCreateOrganizationDialog" />
            <CreateOrganizationDialog ref="createOrganizationDialog" @onCreate="updateOrganizations" />
        </div>
        <div v-if="organizations" class="flex flex-wrap gap-3 mt-4">
            <div v-for="organization in organizations.organizations" class="block-list-item rounded-md bg-white shadow size-fit cursor-pointer h-40"
            @click="navigateTo(`/organization/${organization.id}/`)">
                <span class="text-lg overflow-hidden  whitespace-nowrap text-ellipsis flex flex-col px-5 pb-3 pt-3 gap-1">
                    {{  organization.name }}
                </span>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
const organizationsService = useOrganizationsService();

const createOrganizationDialog = ref();

const organizations = ref();
await updateOrganizations();

function openCreateOrganizationDialog() {
    createOrganizationDialog.value.show();
}

async function updateOrganizations() {
    organizations.value = await organizationsService.getOrganizations();
}
</script>