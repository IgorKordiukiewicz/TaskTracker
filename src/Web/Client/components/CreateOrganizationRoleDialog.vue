<template>
    <ActionDialog header="Create a role" submit-label="Create" @submit="createRole" ref="dialog">
        <LabeledInput label="Name">
            <InputText id="title" v-model="model.name" autocomplete="off" class="w-full" />
        </LabeledInput>
        <LabeledInput label="Permissions">
            <MultiSelect id="title" v-model="selectedPermissions" :options="permissions" option-label="label" class="w-full" />
        </LabeledInput>
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