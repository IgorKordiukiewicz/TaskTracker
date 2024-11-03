<template>
    <ActionDialog header="Create an organization" submit-label="Create" @submit="createOrganization" ref="dialog">
        <div class="flex flex-col gap-1">
            <label for="name">Name</label>
            <InputText id="name" v-model="model.name" autocomplete="off" class="w-full" />
        </div>
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