<template>
    <div class="h-full" v-if="members && roles">
        <div class="flex justify-between items-center">
            <p class="text-lg">Members</p> 
            <div class="flex gap-2 items-center">
                <Button type="button" icon="pi pi-filter-slash" label="Clear" severity="contrast" outlined @click="resetFilters" />
                <IconField>
                    <InputIcon class="pi pi-search" />
                    <InputText v-model="filters['global'].value" placeholder="Search" />
                </IconField>
                <slot />
            </div>
        </div>
        <DataTable :value="members.members" class="mt-4 shadow" paginator :rows="10" :rows-per-page-options="[10, 25, 50]" :always-show-paginator="false"
        removable-sort filter-display="menu" :global-filter-fields="[ 'name', 'email' ]" v-model:filters="filters">
            <Column header="Name" sortable filter-field="name" sort-field="name">
                <template #body="{ data }">
                    <div class="flex gap-4 items-center">
                        <UserAvatar :user-id="data.userId" />
                        <p>{{ data.name }}</p>
                    </div>
                </template>
            </Column>
            <Column field="email" header="Email" sortable></Column>
            <Column field="roleName" header="Role" filter-field="roleName" :show-filter-match-modes="false">
                <template #body="{ data }">
                    <template v-if="canEditMembers">
                        <RoleSelect :roles="roles.roles" :member="data" @on-update="updateMemberRole" />
                    </template>
                    <template v-else>
                        <p>{{ data.roleName }}</p>
                    </template>
                </template>
                <template #filter="{ filterModel }">
                    <MultiSelect v-model="filterModel.value" :options="roles.roles" option-label="name" option-value="name"></MultiSelect>
                </template>
            </Column>
            <Column header="" style="width: 1px;" v-if="canEditMembers">
                <template #body="{ data }">
                    <Button type="button" icon="pi pi-ellipsis-v" text severity="secondary" @click="(e) => toggleMenu(e, data.id)" v-if="!isOwnerRole(data.roleId)"/>
                    <Menu ref="menu" :model="menuItems" :popup="true" />
                </template>             
            </Column>
        </DataTable>
        <ConfirmDialog></ConfirmDialog>
    </div>
</template>

<script setup lang="ts">
import { RemoveMemberDto, UpdateMemberRoleDto } from '~/types/dtos/projects';
import { ProjectPermissions } from '~/types/enums';
import { FilterMatchMode } from '@primevue/core/api';

const route = useRoute();
const projectsService = useProjectsService();
const permissions = usePermissions();
const confirm = useConfirm();

const menu = ref();
const menuItems = ref([
    {
        label: 'Options',
        items: [
            {
                label: 'Remove',
                icon: 'pi pi-trash',
                command: () => {
                    confirm.require({
                        message: `Are you sure you want to remove the member?`,
                        header: 'Confirm action',
                        rejectProps: {
                            label: 'Cancel',
                            severity: 'secondary'
                        },
                        acceptProps: {
                            label: 'Confirm',
                            severity: 'danger'
                        },
                        accept: async () => {
                            const model = new RemoveMemberDto();
                            model.memberId = selectedMemberId.value;
                            await removeMember(model);
                        }
                    })
                }
            }
        ]
    }
])
const selectedMemberId = ref();

const projectId = ref(route.params.id as string);

const filters = ref();
const initFilters = () => {
    filters.value = {
        global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        roleName: { value: null, matchMode: FilterMatchMode.IN }
    }
}
initFilters();

await permissions.checkProjectPermissions(projectId.value);

const members = ref();
const roles = ref(await projectsService.getRoles(projectId.value));
await updateMembers();

const canEditMembers = computed(() => {
    return permissions.hasPermission(ProjectPermissions.EditMembers);
})

function resetFilters() {
    initFilters();
}

function isOwnerRole(roleId: string) {
    return roles.value?.roles.find(x => x.id === roleId)?.owner ?? false;
}

function toggleMenu(event: Event, memberId: string) {
    selectedMemberId.value = memberId;
    menu.value.toggle(event);
}

async function updateMembers() {
    members.value = await projectsService.getMembers(projectId.value);
}

async function updateMemberRole(model: UpdateMemberRoleDto) {
    await projectsService.updateMemberRole(projectId.value, model);
    await updateMembers();
}

async function removeMember(model: RemoveMemberDto) {
    await projectsService.removeMember(projectId.value, model);
    await updateMembers();
}

</script>