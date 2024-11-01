<template>
    <div>
        <p class="text-lg">Projects</p>
        <div v-if="projects" class="flex flex-wrap gap-3 mt-4">
            <div v-for="project in projects.projects" class="project-list-item rounded-md bg-white shadow size-fit cursor-pointer h-40" @click="navigateTo(`/project/${project.id}/`)">
                <span class="text-lg overflow-hidden whitespace-nowrap text-ellipsis flex flex-col px-5 pb-3 pt-3 gap-1">
                    {{ project.name }}
                </span>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
const route = useRoute();
const projectsService = useProjectsService();

const organizationId = ref(route.params.id as string);
const projects = ref(await projectsService.getProjects(organizationId.value));

</script>

<style scoped>
.project-list-item {
    width: calc(20% - 0.6rem);
}
</style>