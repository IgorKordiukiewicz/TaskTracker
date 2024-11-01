<template>
    <div>
        <Breadcrumb :model="items" />
    </div>
</template>

<script setup lang="ts">
const organizationsService = useOrganizationsService();
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
            //{ icon: 'pi pi-home', url: '/' },
            { label: navData.organization.name, url: `/organization/${navData.organization.id}/` },
            { label: navData.project.name, url: `/project/${navData.project.id}/` }
        ];
    }
    else if(route.fullPath.startsWith('/organization')) {
        const id = route.params.id as string;
        const navData = await organizationsService.getNavData(id);
        if(!navData) {
            items.value = [];
            return;
        }
        items.value = [
            //{ icon: 'pi pi-home', url: '/' },
            { label: navData.organization.name, url: `/organization/${navData.organization.id}/` }
        ];
    }
}
</script>

<style scoped>
.p-breadcrumb {
    padding: 0px;
}
</style>