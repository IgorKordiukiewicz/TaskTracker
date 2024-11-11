<template>
    <OrganizationLayout>
        <div class="flex justify-between items-center">
            <p class="text-lg">Roles</p>
            <Button icon="pi pi-plus" severity="primary" label="Create" @click="openCreateRoleDialog" />
            <CreateOrganizationRoleDialog ref="createRoleDialog" :organization-id="organizationId" @on-create="updateRoles" />
        </div>
        <div class="rounded-md bg-white w-100 shadow mt-4 p-4" v-if="roles">
            <table class="w-full" style="border-spacing: 5000px;">
                <tr>
                    <th style="width: 200px;"></th>
                    <th v-for="permission in permissions">
                        {{ permission.label }}
                    </th>
                </tr>
                <tr v-for="role in roles.roles">
                    <td>
                        {{ role.name }}
                    </td>
                    <td v-for="permission in permissions" class="text-center">
                        <Checkbox binary :model-value="hasPermission(permission.value, role.permissions)" :disabled="!role.modifiable" 
                        @change="async (e) => await updateRolePermissions(e, role, permission.value)" />
                    </td>
                </tr>
            </table>
        </div>
    </OrganizationLayout>
</template>

<script setup lang="ts">
import { UpdateOrganizationRolePermissionsDto } from '~/types/dtos/organizations';
import { OrganizationPermissions } from '~/types/enums';
import type { OrganizationRoleVM } from '~/types/viewModels/organizations';

const route = useRoute();
const organizationsService = useOrganizationsService();

const createRoleDialog = ref();

const organizationId = ref(route.params.id as string);
const roles = ref(await organizationsService.getRoles(organizationId.value));

const permissions = ref([
    { label: 'Projects', value: OrganizationPermissions.EditProjects },
    { label: 'Members', value: OrganizationPermissions.EditMembers },
    { label: 'Roles', value: OrganizationPermissions.EditRoles }
])

function hasPermission(permission: OrganizationPermissions, permissions: OrganizationPermissions) {
    return (permissions & permission) === permission;
}

function openCreateRoleDialog() {
    createRoleDialog.value.show();
}

async function updateRoles() {
    roles.value = await organizationsService.getRoles(organizationId.value);
}

async function updateRolePermissions(event: Event, role: OrganizationRoleVM, permission: OrganizationPermissions) {
    const model = new UpdateOrganizationRolePermissionsDto();
    model.roleId = role.id;
    model.permissions = role.permissions ^ permission;
    await organizationsService.updateRolePermissions(organizationId.value, model);
    await updateRoles();
}
</script>

<style scoped>
td {
    padding: 0.75rem 0;
}
</style>