<template>
    <div class="flex justify-between border-b h-14 p-3 items-center">
        <div class="flex gap-2 items-center">
            <i class="pi pi-bars p-2 hover:bg-surface-100 cursor-pointer rounded" @click="toggleSidebar"></i>
            <HierarchyNav></HierarchyNav>
        </div>
        <div class="flex gap-4 items-end">
            <span class="cursor-pointer" style="color: rgb(100, 116, 139);" @click="toggleNotifications">
                <template v-if="invitations && invitations.invitations.length > 0">
                    <OverlayBadge class="cursor-pointer">
                        <i class="pi pi-bell text-xl" />
                    </OverlayBadge>
                </template>
                <template v-else>
                    <i class="pi pi-bell text-xl" />
                </template>
            </span>
            <Popover ref="notificationsPopover">
                <div class="flex flex-col gap-3 w-72" v-if="invitations">
                    <p class="font-semibold">Invitations</p>
                    <div v-for="invitation in invitations.invitations" class="flex justify-between items-center">
                        <p>{{ invitation.organizationName }}</p>
                        <div class="flex gap-1 items-center">
                            <Button icon="pi pi-times" severity="danger" rounded text @click="declineInvitation(invitation.id)" />
                            <Button icon="pi pi-check" severity="success"  rounded text @click="acceptInvitation(invitation.id)" />
                        </div>
                    </div>
                </div>
            </Popover>
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
const notificationsPopover = ref();

const invitations = ref();
await updateInvitations();

const toggle = (event: Event) => {
    menu.value.toggle(event);
}

const toggleNotifications = (event: Event) => {
    if(invitations.value && invitations.value?.invitations.length > 0) {
        notificationsPopover.value.toggle(event);
    }
}

function toggleSidebar() {
    emit('toggleSidebar');
}

async function updateInvitations() {
    invitations.value = await organizationsService.getUserInvitations();
}

async function declineInvitation(id: string) {
    await organizationsService.declineInvitation(id);
    await updateInvitations();
}

async function acceptInvitation(id: string) {
    await organizationsService.acceptInvitation(id);
    await updateInvitations();
    // TODO: update organizations if on index page? (or always redirect to org page)
}
</script>