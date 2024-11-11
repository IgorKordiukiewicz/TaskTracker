<template>
    <ActionDialog header="Update role name" submit-label="Confirm" @submit="updateRoleName" ref="dialog">
        <LabeledInput label="Name">
            <InputText v-model="model.name" autocomplete="off" class="w-full" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { UpdateOrganizationRoleNameDto } from '~/types/dtos/organizations';
import type { OrganizationRoleVM } from '~/types/viewModels/organizations';

defineExpose({ show });
const emit = defineEmits([ 'onUpdate' ]);
const props = defineProps({
    organizationId: { type: String, required: true },
});

const organizationsService = useOrganizationsService();

const dialog = ref();
const model = ref(new UpdateOrganizationRoleNameDto());

function show(role: OrganizationRoleVM) {
    model.value.name = role.name;
    model.value.roleId = role.id;
    dialog.value.show();
}

async function updateRoleName() {
    await organizationsService.updateRoleName(props.organizationId, model.value);

    model.value = new UpdateOrganizationRoleNameDto();

    emit('onUpdate');
}

</script>