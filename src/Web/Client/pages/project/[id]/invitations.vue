<template>
    <div v-if="permissions && canViewPage" class="h-full">
        <div class="flex justify-between items-center">
            <p class="text-lg">Invitations</p>
            <div class="flex gap-2 items-center">
                <Button type="button" icon="pi pi-filter-slash" label="Clear" severity="contrast" outlined @click="resetFilters" />
                <IconField>
                    <InputIcon class="pi pi-search" />
                    <InputText v-model="filters['global'].value" placeholder="Search" />
                </IconField>
                <Button icon="pi pi-plus" severity="primary" label="Invite" @click="openSendInvitationDialog" />
                <SendInvitationDialog ref="sendInvitationDialog" :projectId="projectId" @onCreate="updateInvitations" />
            </div>
        </div>
        <DataTable v-if="invitations" :value="invitations.invitations" class="mt-4 shadow" paginator :rows="10" :rows-per-page-options="[10, 25, 50]" :always-show-paginator="false"
        removable-sort filter-display="menu" :global-filter-fields="['userEmail' ]" v-model:filters="filters">
            <Column header="Email" field="userEmail" sortable></Column>
            <Column header="State" filter-field="state" :show-filter-match-modes="false">
                <template #body="{ data }">
                    <Tag class="w-24" :value="ProjectInvitationState[data.state]" :severity="getStateSeverity(data.state)" v-tooltip.bottom="getExpirationTooltip(data)"></Tag>
                </template>
                <template #filter="{ filterModel }">
                    <MultiSelect v-model="filterModel.value" :options="allInvitationStates" option-label="name" option-value="key"></MultiSelect>
                </template>
            </Column>
            <Column header="Finalized At" sortable sortField="finalizedAt">
                <template #body="{ data }">
                    {{ formatDate(data.finalizedAt) }}
                </template>
            </Column>
            <Column header="Created At" sortable sortField="createdAt">
                <template #body="{ data }">
                    {{ formatDate(data.createdAt) }}
                </template>
            </Column>
            <Column header="" style="width: 10px;">
                <template #body="{ data }">
                    <Button type="button" icon="pi pi-ellipsis-v" text severity="secondary" @click="(e) => toggleMenu(e, data.id)" 
                        :disabled="isMenuButtonDisabled(data.state)" />
                </template>             
        </Column>
        </DataTable>
        <Menu ref="menu" :model="menuItems" :popup="true" />
        <ConfirmDialog></ConfirmDialog>
    </div>
</template>

<script setup lang="ts">
import { FilterMatchMode } from '@primevue/core/api';
import { usePermissions } from '~/stores/permissions';
import { allInvitationStates, ProjectInvitationState, ProjectPermissions } from '~/types/enums';
import type { ProjectInvitationVM } from '~/types/viewModels/projects';

const route = useRoute();
const projectsService = useProjectsService();
const confirm = useConfirm();
const permissions = usePermissions();

const projectId = ref(route.params.id as string);
const invitations = ref();
await updateInvitations();

await permissions.checkProjectPermissions(projectId.value);

const filters = ref();

const sendInvitationDialog = ref();
const selectedInvitation = ref();
const menu = ref();
const menuItems = ref([
    {
        label: 'Options',
        items: [
            {
                label: 'Cancel',
                icon: 'pi pi-times',
                command: () => {
                    const invitationId = selectedInvitation.value;
                    confirm.require({
                        message: `Are you sure you want to cancel the invitation?`,
                        header: 'Confirm action',
                        rejectProps: {
                            label: 'Cancel',
                            severity: 'secondary'
                        },
                        acceptProps: {
                            label: 'Confirm',
                            severity: 'danger'
                        },
                        accept: async () => await cancelInvitation(invitationId)
                    })
                }
            }
        ]
    }
])

const initFilters = () => {
    filters.value = {
        global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        state: { value: null, matchMode: FilterMatchMode.IN }
    }
}
initFilters();

const canViewPage = computed(() => {
    return permissions.hasPermission(ProjectPermissions.EditMembers);
})

function resetFilters() {
    initFilters();
}

function toggleMenu(event: Event, invitationId: string) {
    selectedInvitation.value = invitationId;
    menu.value.toggle(event);
}

function openSendInvitationDialog() {
    sendInvitationDialog.value.show();
}

async function updateInvitations() {
    invitations.value = await projectsService.getInvitations(projectId.value);
}

async function cancelInvitation(invitationId: string) {
    await projectsService.cancelInvitation(projectId.value, invitationId);
    await updateInvitations();
}

function isMenuButtonDisabled(state: ProjectInvitationState) {
    return state != ProjectInvitationState.Pending;
}

function getStateSeverity(state: ProjectInvitationState) {
    switch (+state) {
        case ProjectInvitationState.Pending: 
            return "info";
        case ProjectInvitationState.Accepted:
            return "success";
        case ProjectInvitationState.Canceled:
            return "warn";
        case ProjectInvitationState.Declined:
            return "danger";
        case ProjectInvitationState.Expired:
            return "warn";
        default: 
            return "primary";
    }
}

function formatDate(date?: Date) {
    return date ? new Date(date).toLocaleDateString(undefined, {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    }) : '-';
}

function getExpirationTooltip(data: ProjectInvitationVM) {
    return (data.state == ProjectInvitationState.Pending && data.expirationDate) 
    ? `Expiring at: ${formatDate(data.expirationDate)}`
    : undefined;
}
</script>