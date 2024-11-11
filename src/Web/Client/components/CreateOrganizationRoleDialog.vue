<template>
    <ActionDialog header="Create a role" submit-label="Create" @submit="createRole" ref="dialog">
        <div class="flex flex-col gap-2">
            <div class="flex flex-col gap-1">
                <label for="title">Name</label>
                <InputText id="title" v-model="model.name" autocomplete="off" class="w-full" />
            </div>
            <div class="flex flex-col gap-1">
                <label for="title">Permissions</label>
                <MultiSelect id="title" v-model="selectedPermissions" :options="permissions" option-label="label" class="w-full" />
            </div>
        </div>
    </ActionDialog>
</template>

<script setup lang="ts">
import { CreateOrganizationRoleDto } from '~/types/dtos/organizations';
import { OrganizationPermissions } from '~/types/enums';

defineExpose({ show });
const emit = defineEmits([ 'onCreate' ]);
const props = defineProps({
    organizationId: { type: String, required: true }
});

const organizationsService = useOrganizationsService();

const permissions = ref([
    { label: 'Projects', value: OrganizationPermissions.EditProjects },
    { label: 'Members', value: OrganizationPermissions.EditMembers },
    { label: 'Roles', value: OrganizationPermissions.EditRoles }
])
const selectedPermissions = ref();

const dialog = ref();
const model = ref(new CreateOrganizationRoleDto());

function show() {
    dialog.value.show();
}

async function createRole() {
    let newRolePermissions = OrganizationPermissions.None;
    for(var selectedPermission of selectedPermissions.value) {
        newRolePermissions += selectedPermission.value;
    }
    
    model.value.permissions = newRolePermissions;
    await organizationsService.createRole(props.organizationId, model.value);

    model.value = new CreateOrganizationRoleDto();

    emit('onCreate');
}
</script>