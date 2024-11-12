<template>
    <ActionDialog header="Update role name" submit-label="Confirm" @submit="updateRoleName" ref="dialog">
        <LabeledInput label="Name">
            <InputText v-model="model.name" autocomplete="off" class="w-full" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { UpdateRoleNameDto } from '~/types/dtos/shared';
import type { RoleVM } from '~/types/viewModels/shared';

defineExpose({ show });
const emit = defineEmits([ 'onUpdate' ]);

const dialog = ref();
const model = ref(new UpdateRoleNameDto());

function show(role: RoleVM) {
    model.value.name = role.name;
    model.value.roleId = role.id;
    dialog.value.show();
}

function updateRoleName() {
    emit('onUpdate', model.value);
    model.value = new UpdateRoleNameDto();
}

</script>