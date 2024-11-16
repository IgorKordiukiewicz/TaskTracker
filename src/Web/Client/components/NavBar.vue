<template>
    <ul class="navbar">
        <li v-for="node in nodes">
            <template v-if="node.children">
                <li class="flex justify-between items-center p-3 rounded">
                        <div class="flex gap-3 items-center">
                            <i :class="node.icon" style="height: 16px; color: var(--p-menu-item-icon-color)"></i>
                            <span>{{ node.title }}</span>
                        </div>
                    </li>
                    <li v-for="childNode in node.children" class="ml-3 pl-2" style="border-left: 2px solid #d1d7e0;">
                        <NavBarItem :title="childNode.title" :icon="childNode.icon" :link="childNode.link[0] + id + childNode.link[1]" :include-index="false" v-if="showItem(childNode.permission)" />
                    </li>
            </template>
            <template v-else>
                <NavBarItem :title="node.title" :icon="node.icon" :link="node.link[0] + id + node.link[1]" :include-index="node.includeIndex" v-if="showItem(node.permission)" />
            </template>
        </li>
    </ul>
</template>

<script setup lang="ts">
import { OrganizationPermissions, ProjectPermissions } from '~/types/enums';

const route = useRoute();
const permissions = usePermissions();

const id = computed(() => {
    if(route.path.startsWith('/organization') || route.path.startsWith('/project')) {
        return route.params.id as string;
    }
    else {
        return '';
    }
})

if(route.path.startsWith('/organization')) {
    await permissions.checkOrganizationPermissions(id.value);
}
else if(route.path.startsWith('/project')) {
    await permissions.checkProjectPermissions(id.value);
}

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
                link: [ '/organization/', '/members' ],
            },
            {
                title: 'Invitations',
                icon: 'pi pi-user-plus',
                link: [ '/organization/', '/invitations' ],
                permission: OrganizationPermissions.EditMembers as number
            },
            {
                title: 'Roles',
                icon: 'pi pi-user-edit',
                link: [ '/organization/', '/roles' ],
                permission: OrganizationPermissions.EditRoles as number
            },
        ]
    },
    {
        title: 'Settings',
        icon: 'pi pi-cog',
        link: [ '/organization/', '/settings' ],
        permission: OrganizationPermissions.EditOrganization as number
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
                link: [ '/project/', '/roles' ],
                permission: ProjectPermissions.EditRoles as number
            }
        ]
    },
    {
        title: 'Workflow',
        icon: 'pi pi-arrow-right-arrow-left',
        link: [ '/project/', '/workflow' ],
        permission: ProjectPermissions.EditProject as number
    },
    {
        title: 'Settings',
        icon: 'pi pi-cog',
        link: [ '/project/', '/settings' ],
        permission: ProjectPermissions.EditProject as number
    }
]);

const indexNodes = ref([
    {
        title: 'Dashboard',
        icon: 'pi pi-home',
        includeIndex: true,
        link: [ '', '' ],
        children: null,
        permission: undefined
    }
])

function showItem(permissionRequired?: number) {
    return !permissionRequired || permissions.hasPermission(permissionRequired);
}
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