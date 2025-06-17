<template>
    <div class="flex items-center justify-between">
        <div class="flex items-center gap-3">
            <i :class="icon" />
            <p class="font-semibold">
                {{ title }}
            </p>
        </div>
        <i class="toggle pi pi-chevron-down cursor-pointer" :class="{ 'toggle-open': isOpen }" @click="toggleOpen" style="color: #838fa2;" />
    </div>
    <template v-if="isOpen">
        <slot />
    </template>
</template>

<script setup lang="ts">
const props = defineProps({
    title: { type: String, required: true },
    icon: { type: String, required: true }
})

const isOpen = ref(true);

function toggleOpen() {
    isOpen.value = !isOpen.value;
}
</script>

<style scoped>
.toggle {
    transition-property: transform;
    transition-duration: .3s;
    transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}

.toggle-open {
    transform: rotate(180deg);
}
</style>