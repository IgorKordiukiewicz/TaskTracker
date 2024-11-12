<template>
    <div>
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
    </div>
</template>

<script setup lang="ts">
import type { SelectChangeEvent } from 'primevue/select';
import { UpdateMemberRoleDto } from '~/types/dtos/shared';
import type { ProjectMemberVM } from '~/types/viewModels/projects';

const route = useRoute();
const projectsService = useProjectsService();

const projectId = ref(route.params.id as string);

const members = ref(await projectsService.getMembers(projectId.value));
const roles = ref(await projectsService.getRoles(projectId.value));

const availableRoles = computed(() => {
    return roles.value?.roles.map(x => ({ 
        id: x.id,
        name: x.name
    }));
})

function getRoleValue(member: ProjectMemberVM) {
    return {
        id: member.roleId,
        name: member.roleName
    };
}

async function updateMembers() {
    members.value = await projectsService.getMembers(projectId.value);
}

async function updateMemberRole(event: SelectChangeEvent, member: ProjectMemberVM) {
    const model = new UpdateMemberRoleDto();
    model.memberId = member.id;
    model.roleId = event.value.id;
    await projectsService.updateMemberRole(projectId.value, model);
    await updateMembers();
}

</script>