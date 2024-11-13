<template>
    <ul class="navbar">
        <li v-for="node in nodes">
            <template v-if="node.children">
                <details>
                    <summary class="flex justify-between items-center cursor-pointer p-3 hover:bg-surface-100 rounded">
                        <div class="flex gap-3 items-center">
                            <i :class="node.icon" style="height: 16px; color: var(--p-menu-item-icon-color)"></i>
                            <span>{{ node.title }}</span>
                        </div>
                        <i class="toggle pi pi-chevron-down" style="height: 16px; color: var(--p-menu-item-icon-color)"></i>
                    </summary>
                    <li v-for="childNode in node.children" class="pl-4">
                        <NavBarItem :title="childNode.title" :icon="childNode.icon" :link="childNode.link[0] + id + childNode.link[1]" :include-index="false" />
                    </li>
                </details>
            </template>
            <template v-else>
                <NavBarItem :title="node.title" :icon="node.icon" :link="node.link[0] + id + node.link[1]" :include-index="node.includeIndex" />
            </template>
        </li>
    </ul>
</template>

<script setup lang="ts">
const route = useRoute();

const id = computed(() => {
    if(route.path.startsWith('/organization') || route.path.startsWith('/project')) {
        return route.params.id as string;
    }
    else {
        return '';
    }
})

const nodes = computed(() => {
    if(route.path.startsWith('/organization')) {
        return organizationNodes.value;
    }
    else if(route.path.startsWith('/project')) {
        return projectNodes.value;
    }
    else {
        return indexNodes.value;
    }
})

const organizationNodes = ref([
    {
        title: 'Projects',
        icon: 'pi pi-objects-column', // pi-th-large
        includeIndex: true,
        link: [ '/organization/', '/' ]
    },
    {
        title: 'Team',
        icon: 'pi pi-users',
        children: [
            {
                title: 'Members',
                icon: 'pi pi-user',
                link: [ '/organization/', '/members' ]
            },
            {
                title: 'Invitations',
                icon: 'pi pi-user-plus',
                link: [ '/organization/', '/invitations' ]
            },
            {
                title: 'Roles',
                icon: 'pi pi-user-edit',
                link: [ '/organization/', '/roles' ]
            },
        ]
    }
]);

const projectNodes = ref([
    {
        title: 'Tasks',
        icon: 'pi pi-list',
        includeIndex: true,
        link: [ '/project/', '/' ]
    },
    {
        title: 'Team',
        icon: 'pi pi-users',
        children: [
            {
                title: 'Members',
                icon: 'pi pi-user',
                link: [ '/project/', '/members' ]
            },
            {
                title: 'Roles',
                icon: 'pi pi-user-edit',
                link: [ '/project/', '/roles' ]
            }
        ]
    },
    {
        title: 'Settings',
        icon: 'pi pi-cog',
        link: [ '/project/', '/settings' ]
    }
]);

const indexNodes = ref([
    {
        title: 'Dashboard',
        icon: 'pi pi-home',
        includeIndex: true,
        link: [ '', '' ],
        children: null
    }
])
</script>

<style scoped>

.toggle {
    transition-property: transform;
    transition-duration: .3s;
    transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}

details[open] .toggle {
    transform: rotate(180deg);
}
</style>