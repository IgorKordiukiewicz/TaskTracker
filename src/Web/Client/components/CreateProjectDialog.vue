<template>
    <ActionDialog header="Create a project" submit-label="Create" @submit="createProject" ref="dialog">
        <div class="flex flex-col gap-1">
            <label for="name">Name</label>
            <InputText id="name" v-model="model.name" autocomplete="off" class="w-full" />
        </div>
    </ActionDialog>
</template>

<script setup lang="ts">
import { CreateProjectDto } from '~/types/dtos/projects';

defineExpose({ show });
const emit = defineEmits([ 'onCreate' ]);
const props = defineProps({
    organizationId: { type: String, required: true }
});

const projectsService = useProjectsService();

const dialog = ref();
const model = ref(new CreateProjectDto());

function show() {
    dialog.value.show();
}

async function createProject() {
    model.value.organizationId = props.organizationId;
    await projectsService.createProject(model.value);

    model.value = new CreateProjectDto();

    emit('onCreate');
}
</script>