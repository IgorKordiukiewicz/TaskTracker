<template>
    <Dialog v-model:visible="visible" modal header="Notifications" style="width: 40rem;">
        <Tabs value="0">
            <TabList>
                <Tab value="0">Notifications</Tab>
                <Tab value="1">Invitations</Tab>
            </TabList>
            <TabPanels>
                <TabPanel value="0">
                    <div class="flex flex-col gap-3">
                        <NotificationItem v-for="notification in notifications.notifications" :notification="notification" @on-link-clicked="visible = false"></NotificationItem>
                    </div>
                </TabPanel>
                <TabPanel value="1">
                    <div class="flex flex-col gap-3">
                        <div v-for="invitation in invitations.invitations" class="flex justify-between items-center">
                            <p>{{ invitation.organizationName }}</p>
                            <div class="flex gap-1 items-center">
                                <Button icon="pi pi-times" severity="danger" rounded text @click="declineInvitation(invitation.id)" />
                                <Button icon="pi pi-check" severity="success"  rounded text @click="acceptInvitation(invitation.id)" />
                            </div>
                        </div>
                    </div>
                </TabPanel>
            </TabPanels>
        </Tabs>
    </Dialog>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import type { NotificationsVM } from '~/types/viewModels/notifications';
import type { UserOrganizationInvitationsVM } from '~/types/viewModels/organizations';

defineExpose({ show });
const emit = defineEmits([ 'onInvitationAction' ])
const props = defineProps({
    invitations: { type: Object as PropType<UserOrganizationInvitationsVM>, required: true },
    notifications: { type: Object as PropType<NotificationsVM>, required: true },
})

const organizationsService = useOrganizationsService();
const notificationsService = useNotificationsService();

const visible = ref(false);

function show() {
    visible.value = true;
}

async function declineInvitation(id: string) {
    await organizationsService.declineInvitation(id);
    emit('onInvitationAction');
}

async function acceptInvitation(id: string) {
    await organizationsService.acceptInvitation(id);
    emit('onInvitationAction');
    // TODO: update organizations if on index page? (or always redirect to org page)
}
</script>