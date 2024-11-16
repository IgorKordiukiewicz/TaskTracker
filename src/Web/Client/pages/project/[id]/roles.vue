<template>
    <div v-if="canViewPage">
        <RolesEditor v-if="roles" :roles="roles.roles" :permissions="allPermissions"
        @on-update-role-permissions="updateRolePermissions" @on-update-role-name="updateRoleName" @on-delete-role="deleteRole" @on-create-role="createRole"></RolesEditor>
    </div>
</template>

<script setup lang="ts">
import type { CreateRoleDto, DeleteRoleDto, UpdateRoleNameDto, UpdateRolePermissionsDto } from '~/types/dtos/shared';
import { ProjectPermissions } from '~/types/enums';

const route = useRoute();
const projectsService = useProjectsService();
const permissions = usePermissions();

const projectId = ref(route.params.id as string);
const roles = ref();
await updateRoles();

await permissions.checkProjectPermissions(projectId.value);

const allPermissions = ref([
    { label: 'Tasks', value: ProjectPermissions.EditTasks },
    { label: 'Members', value: ProjectPermissions.EditMembers },
    { label: 'Roles', value: ProjectPermissions.EditRoles },
    { label: 'Project', value: ProjectPermissions.EditProject },
])

const canViewPage = computed(() => {
    return permissions.hasPermission(ProjectPermissions.EditRoles);
})

async function updateRoles() {
    roles.value = await projectsService.getRoles(projectId.value);
}

async function createRole(model: CreateRoleDto) {
    await projectsService.createRole(projectId.value, model);
    await updateRoles();
}

async function updateRolePermissions(model: UpdateRolePermissionsDto) {
    await projectsService.updateRolePermissions(projectId.value, model);
    await updateRoles();
}

async function deleteRole(model: DeleteRoleDto) {
    await projectsService.deleteRole(projectId.value, model);
    await updateRoles();
}

async function updateRoleName(model: UpdateRoleNameDto) {
    await projectsService.updateRoleName(projectId.value, model);
    await updateRoles();
}

</script>