<template>
    <div class="flex justify-between border-b h-14 p-3 items-center">
        <div class="flex gap-2 items-center">
            <i class="pi pi-bars p-2 hover:bg-surface-100 cursor-pointer rounded" @click="toggleSidebar"></i>
            <label v-if="project">{{ project.name }}</label>
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
const projectsService = useProjectsService();
const notificationsService = useNotificationsService();
const route = useRoute();

const project = ref();

watch(() => route.path, checkProjectInfo, { immediate: true });

async function checkProjectInfo() {
    if(route.fullPath.startsWith('/project')) {
        const id = route.params.id as string;
        if(project.value && project.value.id === id) {
            return;
        }
        project.value = await projectsService.getInfo(id);
    }
}

const userId = ref(auth.getUserId());

const menu = ref();
const items = ref([
    { 
        label: 'Profile', 
        icon: 'pi pi-user' ,
        command: async () => {
            await profileDialog.value.show();
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
const notifications = ref();
const unreadCount = ref(await notificationsService.getUnreadCount());

const toggle = (event: Event) => {
    menu.value.toggle(event);
}

const anyNotifications = computed(() => {
    return unreadCount.value ? unreadCount.value > 0 : false;
})

function toggleSidebar() {
    emit('toggleSidebar');
}

async function openNotificationsPopup() {
    if(!anyNotifications.value) {
        return;
    }

    await updateInvitations();
    await updateNotifications();

    notificationsPopup.value.show();
}

async function updateInvitations() {
    invitations.value = await projectsService.getUserInvitations();
}

async function updateNotifications() {
    notifications.value = await notificationsService.getNotifications();
}
</script>