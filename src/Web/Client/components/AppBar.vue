<template>
    <div class="flex justify-between border-b h-14 p-3 items-center">
        <i class="pi pi-bars p-2 hover:bg-surface-100 cursor-pointer rounded" @click="toggleSidebar"></i>
        <div class="flex gap-2 items-center">
            <span @click="toggle">
                <Avatar :label="avatarLabel" shape="circle" aria-haspopup="true" aria-controls="user_menu" class="cursor-pointer" />
            </span>
            <Menu ref="menu" id="user_menu" :popup="true" :model="items" />
        </div>
    </div>
</template>

<script setup lang="ts">
const emit = defineEmits([ 'toggleSidebar' ])

const auth = useAuth();

const menu = ref();
const items = ref([
    { 
        label: 'Profile', 
        icon: 'pi pi-user' 
    },
    { 
        label: 'Logout', 
        icon: 'pi pi-sign-out', 
        command: async () => {
            await auth.logout();
        } 
    }
])

const avatarLabel = computed(() => {
    return auth.getUser()?.email?.at(0)?.toUpperCase() ?? "";
})

const toggle = (event: Event) => {
    menu.value.toggle(event);
}

function toggleSidebar() {
    emit('toggleSidebar');
}
</script>