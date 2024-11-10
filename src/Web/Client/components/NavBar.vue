<template>
    <ul>
        <template v-if="view === 'organization'">
            <NavBarItem label="Projects" :link="getIdLink('organization', '')" icon="pi pi-list" :include-index="true"></NavBarItem>
            <NavBarItem label="Members" :link="getIdLink('organization', 'members')" icon="pi pi-list"></NavBarItem>
        </template>
        <template v-if="view === 'project'">
            <NavBarItem label="Tasks" :link="getIdLink('project', '')" icon="pi pi-list" :include-index="true"></NavBarItem>
            <NavBarItem label="Members" :link="getIdLink('project', 'members')" icon="pi pi-list"></NavBarItem>
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

const id = computed(() => {
    if(route.path.startsWith('/organization') || route.path.startsWith('/project')) {
        return route.params.id as string;
    }
    else {
        return '';
    }
})

function getIdLink(parent: string, page: string) {
    return `/${parent}/${id.value}/${page}`;
}
</script>