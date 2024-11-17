<template>
    <ul class="navbar">
        <li v-for="node in nodes">
            <template v-if="node.children">
                <li class="flex justify-between items-center p-3 rounded">
                        <div class="flex gap-3 items-center">
                            <i :class="node.icon" class="navbar-item-icon"></i>
                            <span>{{ node.title }}</span>
                        </div>
                    </li>
                    <li v-for="childNode in node.children" class="ml-3 pl-2 navbar-child-border">
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

const organizationView = computed(() => {
    return route.path.startsWith('/organization');
})

const projectView = computed(() => {
    return route.path.startsWith('/project');
})

const id = computed(() => {
    if(organizationView || projectView) {
        return route.params.id as string;
    }
    else {
        return '';
    }
})

if(organizationView.value) {
    await permissions.checkOrganizationPermissions(id.value);
}
else if(projectView.value) {
    await permissions.checkProjectPermissions(id.value);
}

const nodes = computed(() => {
    if(organizationView.value) {
        return organizationNodes.value;
    }
    else if(projectView.value) {
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
.navbar-child-border {
    border-left: 2px solid #d1d7e0;
}
</style>