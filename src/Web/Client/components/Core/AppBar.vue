<template>
    <div class="flex justify-between border-b h-14 p-3 items-center">
        <div class="flex gap-2 items-center">
            <i class="pi pi-bars p-2 hover:bg-surface-100 cursor-pointer rounded" @click="toggleSidebar"></i>
            <HierarchyNav></HierarchyNav>
        </div>
        <div class="flex gap-4 items-end">
            <span :class="{ 'cursor-pointer': anyNotifications }" style="color: rgb(100, 116, 139);" @click="openNotificationsPopup">
                <template v-if="anyNotifications">
                    <OverlayBadge class="cursor-pointer">
                        <i class="pi pi-bell text-xl" />
                    </OverlayBadge>
                </template>
                <template v-else>
                    <i class="pi pi-bell text-xl" />
                </template>
            </span>
            <NotificationsPopup ref="notificationsPopup" :invitations="invitations" :notifications="notifications" @on-invitation-action="updateInvitations" @on-notification-read="updateNotifications" />
            <span @click="toggle" v-if="userId">
                <UserAvatar :user-id="userId" aria-haspopup="true" aria-controls="user_menu" class="cursor-pointer" />
            </span>
            <Menu ref="menu" id="user_menu" :popup="true" :model="items" />
            <ProfileDialog ref="profileDialog" />
        </div>
    </div>
</template>

<script setup lang="ts">
const emit = defineEmits([ 'toggleSidebar' ])

const auth = useAuth();
const organizationsService = useOrganizationsService();
const notificationsService = useNotificationsService();

const userId = ref(auth.getUserId());

const menu = ref();
const items = ref([
    { 
        label: 'Profile', 
        icon: 'pi pi-user' ,
        command: () => {
            profileDialog.value.show();
        }
    },
    { 
        label: 'Logout', 
        icon: 'pi pi-sign-out', 
        command: async () => {
            await auth.logout();
        } 
    }
])

const profileDialog = ref();
const notificationsPopup = ref();

const invitations = ref();
await updateInvitations();

const notifications = ref();
await updateNotifications();

const toggle = (event: Event) => {
    menu.value.toggle(event);
}

const anyNotifications = computed(() => {
    return invitations && notifications && (notifications.value.notifications.length > 0 || invitations.value.invitations.length > 0);
})

function toggleSidebar() {
    emit('toggleSidebar');
}

function openNotificationsPopup() {
    if(!anyNotifications.value) {
        return;
    }

    notificationsPopup.value.show();
}

async function updateInvitations() {
    invitations.value = await organizationsService.getUserInvitations();
}

async function updateNotifications() {
    notifications.value = await notificationsService.getNotifications();
}
</script>