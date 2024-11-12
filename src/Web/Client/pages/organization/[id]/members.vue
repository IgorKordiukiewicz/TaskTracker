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
                    <Select :options="availableRoles" option-label="name" :model-value="getRoleValue(slotProps.data)" 
                    @change="async (e) => await updateMemberRole(e, slotProps.data)"  class="w-48" />
                </template>
            </Column>
        </DataTable>
    </OrganizationLayout>
</template>

<script setup lang="ts">
import type { SelectChangeEvent } from 'primevue/select';
import { UpdateOrganizationMemberRoleDto } from '~/types/dtos/organizations';
import { UpdateMemberRoleDto } from '~/types/dtos/shared';
import type { OrganizationMemberVM } from '~/types/viewModels/organizations';

const route = useRoute();
const organizationsService = useOrganizationsService();

const organizationId = ref(route.params.id as string);

const members = ref(await organizationsService.getMembers(organizationId.value));
const roles = ref(await organizationsService.getRoles(organizationId.value));

const availableRoles = computed(() => {
    return roles.value?.roles.map(x => ({ 
        id: x.id,
        name: x.name
    }));
})

function getRoleValue(member: OrganizationMemberVM) {
    return {
        id: member.roleId,
        name: member.roleName
    };
}

async function updateMembers() {
    members.value = await organizationsService.getMembers(organizationId.value);
}

async function updateMemberRole(event: SelectChangeEvent, member: OrganizationMemberVM) {
    const model = new UpdateMemberRoleDto();
    model.memberId = member.id;
    model.roleId = event.value.id;
    await organizationsService.updateMemberRole(organizationId.value, model);
    await updateMembers();
}
</script>