<template>
    <div v-if="roles">
        <div class="flex justify-between items-center">
            <p class="text-lg">Roles</p>
            <Button icon="pi pi-plus" severity="primary" label="Create" @click="openCreateRoleDialog" />
            <CreateRoleDialog :permissions="permissions" @on-create="createRole" ref="createRoleDialog" />
        </div>
        <div class="bg-white w-full shadow mt-4">
            <table class="w-full" style="border-spacing: 5000px;">
                <thead>
                    <tr>
                        <th style="width: 200px;"></th>
                        <th v-for="permission in permissions" class="pb-2">
                            {{ permission.label }}
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="role in props.roles" class="hover:bg-surface-100" @contextmenu="onRightClick($event, role)">
                        <td>
                            {{ role.name }}
                        </td>
                        <td v-for="permission in permissions" class="text-center">
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
import type { PropType } from 'vue';
import { CreateRoleDto, DeleteRoleDto, UpdateRoleNameDto, UpdateRolePermissionsDto } from '~/types/dtos/shared';
import type { RoleVM } from '~/types/viewModels/shared';

const props = defineProps({
    roles: { type: Object as PropType<RoleVM[]>, required: true },
    permissions: { type: Array as PropType<{ label: string, value: number }[]>, required: true }
});

const emit = defineEmits([
    'onUpdateRolePermissions', 'onDeleteRole', 'onUpdateRoleName', 'onCreateRole'
]);

const confirm = useConfirm();

const updateRoleNameDialog = ref();
const createRoleDialog = ref();
const menu = ref();
const selectedRole = ref();

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

function openCreateRoleDialog() {
    createRoleDialog.value.show();
}

function hasPermission(permission: number, permissions: number) {
    return (permissions & permission) === permission;
}

function isSelectedRoleModifiable() {
    return !selectedRole.value?.modifiable;
}

function createRole(model: CreateRoleDto) {
    emit('onCreateRole', model);
}

function updateRolePermissions(event: Event, role: RoleVM, permission: number) {
    const model = new UpdateRolePermissionsDto();
    model.roleId = role.id;
    model.permissions = role.permissions ^ permission;
    emit('onUpdateRolePermissions', model);
}

function deleteRole(roleId: string) {
    const model = new DeleteRoleDto();
    model.roleId = roleId;
    emit('onDeleteRole', model);
}

function updateRoleName(model: UpdateRoleNameDto) {
    emit('onUpdateRoleName', model);
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