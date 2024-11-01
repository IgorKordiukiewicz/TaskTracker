<template>
    <div>
        <p class="text-lg">Organizations</p>
        <div v-if="organizations" class="flex flex-wrap gap-3 mt-4">
            <div v-for="organization in organizations.organizations" class="org-list-item rounded-md bg-white shadow size-fit cursor-pointer h-40"
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

const organizations = ref(await organizationsService.getOrganizations());
</script>

<style scoped>
.org-list-item {
    width: calc(20% - 0.6rem);
}
</style>