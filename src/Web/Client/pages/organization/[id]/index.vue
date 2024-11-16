<template>
    <div v-if="permissions">
        <div class="flex justify-between items-center">
            <p class="text-lg">Projects</p>
            <Button icon="pi pi-plus" severity="primary" label="Create" @click="openCreateProjectDialog" v-if="canCreateProjects" />
            <CreateProjectDialog ref="createProjectDialog" :organizationId="organizationId"@onCreate="updateProjects" />
        </div>
        <div v-if="projects" class="flex flex-wrap gap-3 mt-4">
            <div v-for="project in projects.projects" class="block-list-item rounded-md bg-white shadow size-fit cursor-pointer h-40" @click="navigateTo(`/project/${project.id}/`)">
                <span class="text-lg overflow-hidden whitespace-nowrap text-ellipsis flex flex-col px-5 pb-3 pt-3 gap-1">
                    {{ project.name }}
                </span>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { usePermissions } from '~/stores/permissions';
import { OrganizationPermissions } from '~/types/enums';

const route = useRoute();
const projectsService = useProjectsService();
const permissions = usePermissions();

const createProjectDialog = ref();

const organizationId = ref(route.params.id as string);
const projects = ref();
await updateProjects();

await permissions.checkOrganizationPermissions(organizationId.value);

const canCreateProjects = computed(() => {
    return permissions.hasPermission(OrganizationPermissions.EditProjects);
})

function openCreateProjectDialog() {
    createProjectDialog.value.show();
}

async function updateProjects() {
    projects.value = await projectsService.getProjects(organizationId.value);
}
</script>