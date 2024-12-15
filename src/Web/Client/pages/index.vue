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
                <div class="text-lg overflow-hidden whitespace-nowrap text-ellipsis flex flex-col justify-between px-5 pb-3 pt-3 gap-1 font-medium h-full">
                    {{ organization.name }}
                    <div class="flex gap-3 self-end font-normal">
                        <div class="flex items-center gap-1">
                            <i class="pi pi pi-user" />
                            <p class="text-sm">{{ organization.membersCount }}</p>
                        </div>
                        <div class="flex items-center gap-1">
                            <i class="pi pi-objects-column" />
                            <p class="text-sm">{{ organization.projectsCount }}</p>
                        </div>
                    </div>
                </div>
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