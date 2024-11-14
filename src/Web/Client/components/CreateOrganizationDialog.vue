<template>
    <ActionDialog header="Create an organization" submit-label="Create" @submit="createOrganization" ref="dialog">
        <LabeledInput label="Name">
            <InputText v-model="model.name" autocomplete="off" class="w-full" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { CreateOrganizationDto } from '~/types/dtos/organizations';

defineExpose({ show });
const emit = defineEmits([ 'onCreate' ]);

const organizationsService = useOrganizationsService();

const dialog = ref();
const model = ref(new CreateOrganizationDto());

function show() {
    dialog.value.show();
}

async function createOrganization() {
    await organizationsService.createOrganization(model.value);

    model.value = new CreateOrganizationDto();

    emit('onCreate');
}
</script>