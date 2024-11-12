<template>
    <div>
        <div class="flex justify-between items-center">
            <p class="text-lg">Members</p>
            <Button icon="pi pi-plus" severity="primary" label="Add" @click="openAddProjectMemberDialog" />
            <AddProjectMemberDialog v-if="availableUsers" ref="addProjectMemberDialog" :projectId="projectId" :available-users="availableUsers.users" @on-add="updateMembers" />
        </div>
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
    </div>
</template>

<script setup lang="ts">
import { UpdateMemberRoleDto } from '~/types/dtos/shared';

const route = useRoute();
const projectsService = useProjectsService();
const usersService = useUsersService();

const projectId = ref(route.params.id as string);
const addProjectMemberDialog = ref();

const members = ref(await projectsService.getMembers(projectId.value));
const roles = ref(await projectsService.getRoles(projectId.value));
const availableUsers = ref(await usersService.getAvailableForProject(projectId.value));

async function updateMembers() {
    members.value = await projectsService.getMembers(projectId.value);
    availableUsers.value = await usersService.getAvailableForProject(projectId.value);
}

async function updateMemberRole(model: UpdateMemberRoleDto) {
    await projectsService.updateMemberRole(projectId.value, model);
    await updateMembers();
}

function openAddProjectMemberDialog() {
    addProjectMemberDialog.value.show();
}

</script>