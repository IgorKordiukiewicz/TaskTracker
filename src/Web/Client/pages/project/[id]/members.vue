<template>
    <div class="h-full">
        <MembersList v-if="members && roles" :members="members.members" :roles="roles.roles" :can-edit-members="canEditMembers"
        @on-update-member-role="updateMemberRole" @on-remove-member="removeMember">
            <Button icon="pi pi-plus" severity="primary" label="Add" @click="openAddProjectMemberDialog" v-if="canEditMembers" />
            <AddProjectMemberDialog v-if="availableUsers" ref="addProjectMemberDialog" :projectId="projectId" :available-users="availableUsers.users" @on-add="updateMembers" />
        </MembersList>
    </div>
</template>

<script setup lang="ts">
import { RemoveMemberDto, UpdateMemberRoleDto } from '~/types/dtos/shared';
import { ProjectPermissions } from '~/types/enums';

const route = useRoute();
const projectsService = useProjectsService();
const usersService = useUsersService();
const permissions = usePermissions();

const projectId = ref(route.params.id as string);
const addProjectMemberDialog = ref();

await permissions.checkProjectPermissions(projectId.value);

const members = ref();
const roles = ref(await projectsService.getRoles(projectId.value));
const availableUsers = ref();
await updateMembers();

const canEditMembers = computed(() => {
    return permissions.hasPermission(ProjectPermissions.EditMembers);
})

async function updateMembers() {
    members.value = await projectsService.getMembers(projectId.value);
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