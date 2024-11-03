<template>
    <Dialog v-model:visible="visible" modal header="Create a project" style="min-width: 24rem;">
        <div class="flex flex-col gap-2">
            <div class="flex flex-col gap-1">
                <label for="name">Name</label>
                <InputText id="name" v-model="model.name" autocomplete="off" class="w-full" />
            </div>
            <div class="flex justify-end gap-2 mt-4">
                <Button type="button" label="Cancel" severity="secondary" @click="visible = false" autofocus></Button>
                <Button type="button" label="Save" @click="createProject"></Button>
            </div>
        </div>
    </Dialog>
</template>

<script setup lang="ts">
import { CreateProjectDto } from '~/types/dtos/projects';

defineExpose({ show });
const emit = defineEmits([ 'onCreate' ]);
const props = defineProps({
    organizationId: { type: String, required: true }
});

const projectsService = useProjectsService();

const visible = ref(false);
const model = ref(new CreateProjectDto());

function show() {
    visible.value = true;
}

async function createProject() {
    model.value.organizationId = props.organizationId;
    await projectsService.createProject(model.value);

    model.value = new CreateProjectDto();

    visible.value = false;
    emit('onCreate');
}
</script>