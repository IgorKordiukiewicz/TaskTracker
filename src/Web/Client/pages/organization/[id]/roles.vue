<template>
    <OrganizationLayout>
        <div class="flex justify-between items-center">
            <p class="text-lg">Roles</p>
            <Button icon="pi pi-plus" severity="primary" label="Create" @click="openCreateRoleDialog" />
            <CreateOrganizationRoleDialog ref="createRoleDialog" :organization-id="organizationId" @on-create="updateRoles" />
        </div>
        <div class="rounded-md bg-white w-100 shadow mt-4 py-4" v-if="roles">
            <table class="w-full" style="border-spacing: 5000px;">
                <tr>
                    <th style="width: 200px;"></th>
                    <th v-for="permission in permissions" class="pb-2">
                        {{ permission.label }}
                    </th>
                </tr>
                <tr v-for="role in roles.roles" class="hover:bg-surface-100" @contextmenu="onRightClick($event, role)">
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
        <ContextMenu ref="menu" :model="menuItems" @hide="selectedRole = null" />
        <UpdateOrganizationRoleNameDialog ref="updateRoleNameDialog" :organization-id="organizationId" @on-update="updateRoles" />
        <ConfirmDialog></ConfirmDialog>
    </OrganizationLayout>
</template>

<script setup lang="ts">
import { DeleteOrganizationRoleDto, UpdateOrganizationRolePermissionsDto } from '~/types/dtos/organizations';
import { OrganizationPermissions } from '~/types/enums';
import type { OrganizationRoleVM } from '~/types/viewModels/organizations';

const route = useRoute();
const organizationsService = useOrganizationsService();
const confirm = useConfirm();

const createRoleDialog = ref();
const updateRoleNameDialog = ref();
const menu = ref();
const selectedRole = ref();

const organizationId = ref(route.params.id as string);
const roles = ref(await organizationsService.getRoles(organizationId.value));

const permissions = ref([
    { label: 'Projects', value: OrganizationPermissions.EditProjects },
    { label: 'Members', value: OrganizationPermissions.EditMembers },
    { label: 'Roles', value: OrganizationPermissions.EditRoles }
])

const menuItems = ref([
    {
        label: 'Edit name',
        icon: 'pi pi-pencil',
        command: () => {
            updateRoleNameDialog.value.show(selectedRole.value);
        }
    },
    {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => {
            const roleId = selectedRole.value.id;
            confirm.require({
                message: `Are you sure you want to delete the ${selectedRole.value.name} role?`,
                header: 'Confirm action',
                rejectProps: {
                    label: 'Cancel',
                    severity: 'secondary'
                },
                acceptProps: {
                    label: 'Confirm',
                    severity: 'danger'
                },
                accept: async () => await deleteRole(roleId)
            })
        }
    }
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

async function deleteRole(roleId: string) {
    const model = new DeleteOrganizationRoleDto();
    model.roleId = roleId;
    await organizationsService.deleteRole(organizationId.value, model);
    await updateRoles();
}

function onRightClick(event: Event, role: OrganizationRoleVM) {
    selectedRole.value = role;
    menu.value.show(event);
}
</script>

<style scoped>
td {
    padding: 0.75rem 1rem;
    border-style: solid;
    border-width: 0 0 1px 0;
}
</style>