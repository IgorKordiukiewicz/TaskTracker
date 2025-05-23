<template>
    <div v-if="canViewPage && roles">
        <div class="flex justify-between items-center">
            <p class="text-lg">Roles</p>
            <Button icon="pi pi-plus" severity="primary" label="Create" @click="openCreateRoleDialog" />
            <CreateRoleDialog :permissions="allPermissions" @on-create="createRole" ref="createRoleDialog" />
        </div>
        <div class="bg-white w-full shadow mt-4">
            <table class="w-full">
                <thead>
                    <tr>
                        <th class="w-40"></th>
                        <th v-for="permission in allPermissions" class="pb-2">
                            {{ permission.label }}
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="role in roles.roles" class="hover:bg-surface-100" @contextmenu="onRightClick($event, role)">
                        <td>
                            {{ role.name }}
                        </td>
                        <td v-for="permission in allPermissions" class="text-center">
                            <Checkbox binary :model-value="hasPermission(permission.value, role.permissions)" :disabled="!role.modifiable" 
                            @change="async (e: Event) => await updateRolePermissions(e, role, permission.value)" />
                        </td>
                    </tr>
                </tbody>

            </table>
        </div>
        <ContextMenu ref="menu" :model="menuItems" @hide="selectedRole = null" />
        <UpdateRoleNameDialog ref="updateRoleNameDialog" @on-update="updateRoleName" />
        <ConfirmDialog></ConfirmDialog>
    </div>
</template>

<script setup lang="ts">
import { DeleteRoleDto, UpdateRolePermissionsDto, type CreateRoleDto, type UpdateRoleNameDto } from '~/types/dtos/projects';
import { ProjectPermissions } from '~/types/enums';
import type { RoleVM } from '~/types/viewModels/projects';

const route = useRoute();
const projectsService = useProjectsService();
const permissions = usePermissions();
const confirm = useConfirm();

const updateRoleNameDialog = ref();
const createRoleDialog = ref();
const menu = ref();
const selectedRole = ref();
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

const menuItems = ref([
    {
        label: 'Edit name',
        icon: 'pi pi-pencil',
        disabled: isSelectedRoleModifiable,
        command: () => {
            updateRoleNameDialog.value.show(selectedRole.value);
        }
    },
    {
        label: 'Delete',
        icon: 'pi pi-trash',
        disabled: isSelectedRoleModifiable,
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


const canViewPage = computed(() => {
    return permissions.hasPermission(ProjectPermissions.EditRoles);
})

function openCreateRoleDialog() {
    createRoleDialog.value.show();
}

function hasPermission(permission: number, permissions: number) {
    return (permissions & permission) === permission;
}

function isSelectedRoleModifiable() {
    return !selectedRole.value?.modifiable;
}

async function updateRoles() {
    roles.value = await projectsService.getRoles(projectId.value);
}

async function createRole(model: CreateRoleDto) {
    await projectsService.createRole(projectId.value, model);
    await updateRoles();
}

async function updateRolePermissions(event: Event, role: RoleVM, permission: number) {
    const model = new UpdateRolePermissionsDto();
    model.roleId = role.id;
    model.permissions = role.permissions ^ permission;
    await projectsService.updateRolePermissions(projectId.value, model);
    await updateRoles();
}

async function deleteRole(roleId: string) {
    const model = new DeleteRoleDto();
    model.roleId = roleId;
    await projectsService.deleteRole(projectId.value, model);
    await updateRoles();
}

async function updateRoleName(model: UpdateRoleNameDto) {
    await projectsService.updateRoleName(projectId.value, model);
    await updateRoles();
}

function onRightClick(event: Event, role: RoleVM) {
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

th {
    padding: 0.75rem 1rem;
}
</style>