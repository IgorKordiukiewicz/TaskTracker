<template>
    <div class="flex">
        <div class="w-80 h-screen border-e-2 p-2 flex flex-col" v-if="sidebarVisible">
            <p class="text-xl text-center mb-4 font-medium cursor-pointer" @click="navigateTo('/')">Task Tracker</p>
            <NavBar />
        </div>
        <div class="w-full max-h-screen">
            <AppBar @toggle-sidebar="toggleSidebar" />
            <Toast />
            <div class="bg-slate-100 p-4 overflow-y-auto" style="height: calc(100vh - 3.5rem);">
                <slot />        
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
const sidebarVisible = ref(true);

function toggleSidebar() {
    sidebarVisible.value = !sidebarVisible.value;
}
</script>