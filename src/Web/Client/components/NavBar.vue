<template>
    <ul>
        <template v-if="view === 'organization'">
            <NavBarItem label="Projects" link="/" icon="pi pi-list" :include-index="true"></NavBarItem>
        </template>
        <template v-if="view === 'project'">
            <NavBarItem label="Tasks" link="/" icon="pi pi-list" :include-index="true"></NavBarItem>
        </template>
        <template v-if="view === 'default'">
            <NavBarItem label="Organizations" link="/" icon="pi pi-list" :include-index="true"></NavBarItem>
        </template>
        
    </ul>
</template>

<script setup lang="ts">
const route = useRoute();

const view = computed(() => {
    if(route.path.startsWith('/organization')) {
        return 'organization';
    }
    else if(route.path.startsWith('/project')) {
        return 'project';
    }
    else {
        return 'default';
    }
})
</script>