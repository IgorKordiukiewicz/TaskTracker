<template>
    <div>
        <Breadcrumb :model="items" />
    </div>
</template>

<script setup lang="ts">
const projectsService = useProjectsService();
const route = useRoute();

const items = ref();

watch(() => route.path, getBreadcrumbs, { immediate: true });

async function getBreadcrumbs() {
    items.value = [];
    if(route.fullPath.startsWith('/project')) {
        const id = route.params.id as string;
        const navData = await projectsService.getNavData(id);
        if(!navData) {
            items.value = [];
            return;
        }
        items.value = [
            { label: navData.project.name, url: `/project/${navData.project.id}/` }
        ];
    }
}
</script>

<style scoped>
.p-breadcrumb {
    padding: 0px;
}
</style>