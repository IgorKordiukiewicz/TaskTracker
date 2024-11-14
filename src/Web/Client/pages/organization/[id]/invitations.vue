<template>
    <OrganizationLayout>
        <div class="flex justify-between items-center">
            <p class="text-lg">Invitations</p>
            <Button icon="pi pi-plus" severity="primary" label="Invite" @click="openSendInvitationDialog" />
            <SendInvitationDialog ref="sendInvitationDialog" :organizationId="organizationId" @onCreate="updateInvitations" />
        </div>
        <DataTable v-if="invitations" :value="invitations.invitations" class="mt-4 shadow">
            <Column header="Email" field="userEmail"></Column>
            <Column header="Finalized At">
                <template #body="slotProps">
                    {{ formatDate(slotProps.data.finalizedAt) }}
                </template>
            </Column>
            <Column header="State">
                <template #body="slotProps">
                    <Tag class="w-24" :value="OrganizationInvitationState[slotProps.data.state]" :severity="getStateSeverity(slotProps.data.state)"></Tag>
                </template>
            </Column>
            <Column header="Created At" >
                <template #body="slotProps">
                    {{ formatDate(slotProps.data.createdAt) }}
                </template>
            </Column>
            <Column header="" style="width: 10px;">
                <template #body="slotProps">
                    <Button type="button" icon="pi pi-ellipsis-v" text severity="secondary" @click="(e) => toggleMenu(e, slotProps.data.id)" 
                        :disabled="isMenuButtonDisabled(slotProps.data.state)" />
                </template>             
        </Column>
        </DataTable>
        <Menu ref="menu" :model="menuItems" :popup="true" />
        <ConfirmDialog></ConfirmDialog>
    </OrganizationLayout>
</template>

<script setup lang="ts">
import { OrganizationInvitationState } from '~/types/enums';

const route = useRoute();
const organizationsService = useOrganizationsService();
const confirm = useConfirm();

const organizationId = ref(route.params.id as string);
const invitations = ref(await organizationsService.getInvitations(organizationId.value));

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

function toggleMenu(event: Event, invitationId: string) {
    selectedInvitation.value = invitationId;
    menu.value.toggle(event);
}

function openSendInvitationDialog() {
    sendInvitationDialog.value.show();
}

async function updateInvitations() {
    invitations.value = await organizationsService.getInvitations(organizationId.value);
}

async function cancelInvitation(invitationId: string) {
    await organizationsService.cancelInvitation(organizationId.value, invitationId);
    await updateInvitations();
}

function isMenuButtonDisabled(state: OrganizationInvitationState) {
    return state != OrganizationInvitationState.Pending;
}

function getStateSeverity(state: OrganizationInvitationState) {
    switch (+state) {
        case OrganizationInvitationState.Pending: 
            return "info";
        case OrganizationInvitationState.Accepted:
            return "success";
        case OrganizationInvitationState.Canceled:
            return "warn";
        case OrganizationInvitationState.Declined:
            return "danger";
        default: 
            return "primary";
    }
}

function formatDate(date?: Date) {
    return date ? new Date(date).toLocaleDateString() : '-';
}
</script>