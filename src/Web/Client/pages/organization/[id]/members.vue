<template>
    <OrganizationLayout>
        <p class="text-lg">Members</p>
        <DataTable :value="members.members" v-if="members && roles" class="mt-4 shadow">
            <Column header="Name">
                <template #body="slotProps">
                    <div class="flex gap-4 items-center">
                        <Avatar label="AA" shape="circle" /> <!--TODO-->
                        <p>{{ slotProps.data.name }}</p>
                    </div>
                </template>
            </Column>
            <Column field="email" header="Email"></Column>
            <Column field="roleName" header="Role">
                <template #body="slotProps">
                    <RoleSelect :roles="roles.roles" :member="slotProps.data" @on-update="updateMemberRole" />
                </template>
            </Column>
        </DataTable>
    </OrganizationLayout>
</template>

<script setup lang="ts">
import { UpdateMemberRoleDto } from '~/types/dtos/shared';

const route = useRoute();
const organizationsService = useOrganizationsService();

const organizationId = ref(route.params.id as string);

const members = ref(await organizationsService.getMembers(organizationId.value));
const roles = ref(await organizationsService.getRoles(organizationId.value));

async function updateMembers() {
    members.value = await organizationsService.getMembers(organizationId.value);
}

async function updateMemberRole(model: UpdateMemberRoleDto) {
    await organizationsService.updateMemberRole(organizationId.value, model);
    await updateMembers();
}
</script>