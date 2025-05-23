<template>
    <div>
        <div class="flex justify-between items-center">
            <p class="text-lg">Projects</p>
            <Button icon="pi pi-plus" severity="primary" label="Create" @click="openCreateProjectDialog" />
            <CreateProjectDialog ref="createProjectDialog" @onCreate="updateProjects" />
        </div>
        <div v-if="projects" class="flex flex-wrap gap-3 mt-4">
            <div v-for="project in projects.projects" class="block-list-item rounded-md bg-white shadow size-fit cursor-pointer h-40" @click="navigateTo(`/project/${project.id}/`)">
                <div class="text-lg overflow-hidden whitespace-nowrap text-ellipsis flex flex-col justify-between px-5 pb-3 pt-3 gap-1 font-medium h-full">
                    {{ project.name }}
                    <div class="flex gap-3 self-end font-normal">
                        <div class="flex items-center gap-1">
                            <i class="pi pi pi-user" />
                            <p class="text-sm">{{ project.membersCount }}</p>
                        </div>
                        <div class="flex items-center gap-1">
                            <i class="pi pi-check-circle" />
                            <p class="text-sm">{{ project.tasksCount }}</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
const projectsService = useProjectsService();

const createProjectDialog = ref();

const projects = ref();
await updateProjects();

function openCreateProjectDialog() {
    createProjectDialog.value.show();
}

async function updateProjects() {
    projects.value = await projectsService.getProjects();
}
</script>