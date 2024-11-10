<template>
    <OrganizationLayout>
        <p class="text-lg">Members</p>
        <div v-if="members && roles" class="rounded-md bg-white w-100 shadow mt-4 p-4">
            <DataTable :value="members.members" v-if="members">
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
        </div>
    </OrganizationLayout>
</template>

<script setup lang="ts">
import type { SelectChangeEvent } from 'primevue/select';
import { UpdateOrganizationMemberRoleDto } from '~/types/dtos/organizations';
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
    const model = new UpdateOrganizationMemberRoleDto();
    model.memberId = member.id;
    model.roleId = event.value.id;
    await organizationsService.updateMemberRole(organizationId.value, model);
    await updateMembers();
}
</script>