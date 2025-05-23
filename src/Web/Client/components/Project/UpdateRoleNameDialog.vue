<template>
    <ActionDialog header="Update role name" submit-label="Confirm" @submit="updateRoleName" ref="dialog" :submit-disabled="submitDisabled">
        <LabeledInput label="Name">
            <InputText v-model="model.name" autocomplete="off" class="w-full" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { UpdateRoleNameDto } from '~/types/dtos/projects';
import type { RoleVM } from '~/types/viewModels/projects';

defineExpose({ show });
const emit = defineEmits([ 'onUpdate' ]);

const dialog = ref();
const model = ref(new UpdateRoleNameDto());
const initialName = ref();

const submitDisabled = computed(() => {
    return !model.value.name || model.value.name === initialName.value;
})

function show(role: RoleVM) {
    initialName.value = role.name;
    model.value.name = role.name;
    model.value.roleId = role.id;
    dialog.value.show();
}

function updateRoleName() {
    emit('onUpdate', model.value);
    model.value = new UpdateRoleNameDto();
}

</script>