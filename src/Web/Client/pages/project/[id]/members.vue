<template>
    <div>
        <div class="flex justify-between items-center">
            <p class="text-lg">Members</p>
            <Button icon="pi pi-plus" severity="primary" label="Add" @click="openAddProjectMemberDialog" />
            <AddProjectMemberDialog v-if="availableUsers" ref="addProjectMemberDialog" :projectId="projectId" :available-users="availableUsers.users" @on-add="updateMembers" />
        </div>
        <MembersList v-if="members && roles" :members="members.members" :roles="roles.roles" 
        @on-update-member-role="updateMemberRole" @on-remove-member="removeMember" />
    </div>
</template>

<script setup lang="ts">
import { RemoveMemberDto, UpdateMemberRoleDto } from '~/types/dtos/shared';

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

async function removeMember(model: RemoveMemberDto) {
    await projectsService.removeMember(projectId.value, model);
    await updateMembers();
}

function openAddProjectMemberDialog() {
    addProjectMemberDialog.value.show();
}

</script>