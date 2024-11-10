<template>
    <OrganizationLayout>
        <div class="flex justify-between items-center">
            <p class="text-lg">Invitations</p>
            <Button icon="pi pi-plus" severity="primary" label="Invite" @click="openSendInvitationDialog" />
            <SendInvitationDialog ref="sendInvitationDialog" :organizationId="organizationId" @onCreate="updateInvitations" />
        </div>
        <div v-if="invitations" class="rounded-md bg-white w-100 shadow mt-4 p-4">
            <DataTable :value="invitations.invitations">
                <Column header="Email" field="userEmail"></Column>
                <Column header="Finalized At">
                    <template #body="slotProps">
                        {{ formatDate(slotProps.data.finalizedAt) }}
                    </template>
                </Column>
                <Column header="State">
                    <template #body="slotProps">
                        <div class="rounded-full py-2 px-4 w-28 flex justify-center text-white" :style="{ 'background': getStateColor(slotProps.data.state) }" >
                            {{ OrganizationInvitationState[slotProps.data.state] }}
                        </div>
                    </template>
                </Column>
                <Column header="Created At" >
                    <template #body="slotProps">
                        {{ formatDate(slotProps.data.createdAt) }}
                    </template>
                </Column>
            </DataTable>
        </div>
    </OrganizationLayout>
</template>

<script setup lang="ts">
import { OrganizationInvitationState } from '~/types/enums';

const route = useRoute();
const organizationsService = useOrganizationsService();

const sendInvitationDialog = ref();

const organizationId = ref(route.params.id as string);
const invitations = ref(await organizationsService.getInvitations(organizationId.value));

function openSendInvitationDialog() {
    sendInvitationDialog.value.show();
}

async function updateInvitations() {
    invitations.value = await organizationsService.getInvitations(organizationId.value);
}

function getStateColor(state: OrganizationInvitationState) {
    switch (+state) {
        case OrganizationInvitationState.Pending: 
            return "var(--p-button-text-info-color)";
        case OrganizationInvitationState.Accepted:
            return "var(--p-button-text-success-color)";
        case OrganizationInvitationState.Canceled:
            return "var(--p-button-text-warn-color)";
        case OrganizationInvitationState.Declined:
            return "var(--p-button-text-danger-color)";
        default: 
            return "#ffffff";
    }
}

function formatDate(date?: Date) {
    return date ? new Date(date).toLocaleString() : '-';
}
</script>