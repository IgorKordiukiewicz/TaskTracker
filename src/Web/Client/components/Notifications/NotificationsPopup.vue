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
                        <template v-if="notifications.notifications.length > 0">
                            <NotificationItem v-for="notification in notifications.notifications" :notification="notification" @on-link-clicked="visible = false" @on-read="emit('onNotificationRead')"></NotificationItem>
                        </template>
                        <template v-else>
                            <p>You don't have any unread notifications.</p>
                        </template>
                    </div>
                </TabPanel>
                <TabPanel value="1">
                    <div class="flex flex-col gap-3">
                        <template v-if="invitations.invitations.length > 0">
                            <div v-for="invitation in invitations.invitations" class="flex justify-between items-center">
                                <p>{{ invitation.projectName }}</p>
                                <div class="flex gap-1 items-center">
                                    <Button icon="pi pi-times" severity="danger" rounded text @click="declineInvitation(invitation.id)" />
                                    <Button icon="pi pi-check" severity="success"  rounded text @click="acceptInvitation(invitation.id)" />
                                </div>
                            </div>
                        </template>
                        <template v-else>
                            <p>You don't have any invitations.</p>
                        </template>
                    </div>
                </TabPanel>
            </TabPanels>
        </Tabs>
    </Dialog>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import type { NotificationsVM } from '~/types/viewModels/notifications';
import type { UserProjectInvitationsVM } from '~/types/viewModels/projects';

defineExpose({ show });
const emit = defineEmits([ 'onInvitationAction', 'onNotificationRead' ])
const props = defineProps({
    invitations: { type: Object as PropType<UserProjectInvitationsVM>, required: true },
    notifications: { type: Object as PropType<NotificationsVM>, required: true },
})

const projectsService = useProjectsService();
const usersPresentationData = useUsersPresentationData();

const visible = ref(false);

function show() {
    visible.value = true;
}

async function declineInvitation(id: string) {
    await projectsService.declineInvitation(id);
    closeIfNoNotificationsLeft();
    emit('onInvitationAction');
}

async function acceptInvitation(id: string) {
    const projectId = props.invitations.invitations.find(x => x.id === x.id)!.projectId;

    await projectsService.acceptInvitation(id);
    closeIfNoNotificationsLeft();
    emit('onInvitationAction');

    await usersPresentationData.reset();

    navigateTo(`/project/${projectId}/`);
}

function closeIfNoNotificationsLeft() {
    const anyNotificationsLeft = props.notifications.notifications.length > 0 || props.invitations.invitations.length > 1;
    if(!anyNotificationsLeft) {
        visible.value = false;
    }
}
</script>