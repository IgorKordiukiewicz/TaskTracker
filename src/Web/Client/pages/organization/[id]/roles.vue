<template>
    <div v-if="canViewPage">
        <RolesEditor v-if="roles" :roles="roles.roles" :permissions="allPermissions" 
        @on-update-role-permissions="updateRolePermissions" @on-update-role-name="updateRoleName" @on-delete-role="deleteRole" @on-create-role="createRole"></RolesEditor>
    </div>
</template>

<script setup lang="ts">
import type { CreateRoleDto, DeleteRoleDto, UpdateRoleNameDto, UpdateRolePermissionsDto } from '~/types/dtos/shared';
import { OrganizationPermissions } from '~/types/enums';

const route = useRoute();
const organizationsService = useOrganizationsService();
const permissions = usePermissions();

const organizationId = ref(route.params.id as string);
const roles = ref(await organizationsService.getRoles(organizationId.value));

await permissions.checkOrganizationPermissions(organizationId.value);

const allPermissions = ref([
    { label: 'Projects', value: OrganizationPermissions.EditProjects },
    { label: 'Members', value: OrganizationPermissions.EditMembers },
    { label: 'Roles', value: OrganizationPermissions.EditRoles }
])

const canViewPage = computed(() => {
    return permissions.hasPermission(OrganizationPermissions.EditRoles);
})

async function updateRoles() {
    roles.value = await organizationsService.getRoles(organizationId.value);
}

async function createRole(model: CreateRoleDto) {
    await organizationsService.createRole(organizationId.value, model);
    await updateRoles();
}

async function updateRolePermissions(model: UpdateRolePermissionsDto) {
    await organizationsService.updateRolePermissions(organizationId.value, model);
    await updateRoles();
}

async function deleteRole(model: DeleteRoleDto) {
    await organizationsService.deleteRole(organizationId.value, model);
    await updateRoles();
}

async function updateRoleName(model: UpdateRoleNameDto) {
    await organizationsService.updateRoleName(organizationId.value, model);
    await updateRoles();
}
</script>