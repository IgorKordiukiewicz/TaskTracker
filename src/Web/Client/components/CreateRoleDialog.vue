<template>
    <ActionDialog header="Create a role" submit-label="Create" @submit="createRole" ref="dialog" :submit-disabled="submitDisabled">
        <LabeledInput label="Name">
            <InputText id="title" v-model="model.name" autocomplete="off" class="w-full" />
        </LabeledInput>
        <LabeledInput label="Permissions">
            <MultiSelect id="title" v-model="selectedPermissions" :options="props.permissions" option-label="label" class="w-full" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { CreateRoleDto } from '~/types/dtos/shared';

defineExpose({ show });
const emit = defineEmits([ 'onCreate' ]);
const props = defineProps({
    permissions: { type: Array as PropType<{ label: string, value: number }[]>, required: true }
});

const selectedPermissions = ref();

const dialog = ref();
const model = ref(new CreateRoleDto());

const submitDisabled = computed(() => {
    return !model.value.name;
})

function show() {
    dialog.value.show();
}

async function createRole() {
    let newRolePermissions = 0;
    for(var selectedPermission of selectedPermissions.value) {
        newRolePermissions += selectedPermission.value;
    }
    model.value.permissions = newRolePermissions;

    emit('onCreate', model.value);

    model.value = new CreateRoleDto();
}
</script>